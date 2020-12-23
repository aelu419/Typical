using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapedNoise : MonoBehaviour
{
    private SimplePerlinTexture perlin;
    //private Texture2D strokeTex, swirlTex, noiseTex;
    public Texture2D mainTex;
    public texPainterTypes painterType;

    public float freq; //repetitiveness of the texture itself
    public float speed; //speed of evolution of the texture
    public float cursorSpeed; //for random sampling purposes
    public int resolution; //size of the texture

    public enum texPainterTypes
    {
        image,
        stretch,
        rotate
    }

    private Material mat;

    private float t = 0;
    private float looper = 0;
    private Vector2 cursor, cursor1;
    private Perlin cursorMover;

    private Painter texPainter;

    // Start is called before the first frame update
    void OnEnable()
    {
        mat = GetComponent<MeshRenderer>().material;
        perlin = new SimplePerlinTexture(8, 32, speed);
        perlin.FillTexture(0);
        mat.SetTexture("_NoiseTex", perlin.tex);

        switch (painterType)
        {
            case (texPainterTypes.image):
                texPainter = new IdlePainter();
                mat.SetTexture("_MainTex", mainTex);
                break;
            case (texPainterTypes.stretch):
                texPainter = new StretchPainter(resolution, speed, freq);
                mat.SetTexture("_MainTex", texPainter.tex);
                break;
            default:
                throw new System.Exception("Invalid painter type");
        }

        mat.SetVector("_DevBrush0", new Vector2(Random.value, Random.value));
        mat.SetVector("_DevBrush1", new Vector2(Random.value, Random.value));

        cursor = new Vector2(0, 0);
        cursor = new Vector2(0, 0);
        cursorMover = new Perlin(10, 2);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        perlin.FillTexture(t, 0.0f, 1.0f);

        texPainter.FillTexture(t, 0.15f, 0.3f);

        looper += Time.deltaTime * speed;

        bool breakLoop = looper > 1;
        looper = looper % 1;

        float master1 = cursorMover.Noise(new VecN(new float[] {t * 0.01f, 1}));
        cursor.x += Time.deltaTime * cursorSpeed * master1 * (cursorMover.Noise(new VecN(new float[] {
            cursor1.x * 10,
            t
            })) - 0.5f);
        cursor.y += Time.deltaTime * cursorSpeed * master1 * (cursorMover.Noise(new VecN(new float[] {
            t,
            cursor1.y * 10,
            })) - 0.5f);

        float master2 = cursorMover.Noise(new VecN(new float[] { 3 + t * 0.01f, 1 }));
        cursor1.x += Time.deltaTime * cursorSpeed * master2 * (cursorMover.Noise(new VecN(new float[] {
            (1 - cursor.x) * 10,
            t
            })) - 0.5f);
        cursor1.y += Time.deltaTime * cursorSpeed * master2 * (cursorMover.Noise(new VecN(new float[] {
            (cursor.y) * 10,
            t
            })) - 0.5f);

        mat.SetVector("_DevStroke", cursor);
        mat.SetVector("_DevSwirl", cursor1);

        if (breakLoop)
        {
            mat.SetVector("_DevBrush0", mat.GetVector("_DevBrush1"));
            mat.SetVector("_DevBrush1", new Vector2(Random.value, Random.value));
        }

        //Debug.Log(looper);
        mat.SetFloat("_BrushPrgs", looper);
    }
}
