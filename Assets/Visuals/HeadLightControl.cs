using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLightControl : MonoBehaviour
{

    public bool light;

    //intensity, fall off intensity, inner radius, outer radius

    public Vector4 light_min_max_off;
    public Vector4 light_min_max_on;

    [ReadOnly] public Vector4 current_setting;
    [ReadOnly] public bool direction; //true is right

    private UnityEngine.Experimental.Rendering.Universal.Light2D head_light;
    private float speed = 5;

    private Vector4 right_direction;
    private Vector4 left_direction;

    // Start is called before the first frame update
    void Start()
    {
        light = false;
        head_light = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();

        right_direction = new Vector4(
            transform.localPosition.x,
            transform.localPosition.y,
            transform.localPosition.z,
            transform.rotation.eulerAngles.z
            ) ;

        left_direction = right_direction * -1;
        //vertical position of light remains the same during flipping
        left_direction.y = right_direction.y;
    }

    // Update is called once per frame
    void Update()
    {
        float perturbation;
        if (light)
        {
            perturbation = 0.2f;
            current_setting = light_min_max_on;
        }
        else
        {
            perturbation = 0.6f;
            current_setting = light_min_max_off;
        }

        if (direction)
        {
            transform.localPosition = new Vector3(
                right_direction.x,
                right_direction.y,
                right_direction.z);
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, right_direction.w));
        }
        else
        {
            transform.localPosition = new Vector3(
                left_direction.x,
                left_direction.y,
                left_direction.z);
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, left_direction.w));
        }

        head_light.intensity = current_setting.x * (1 + perturbation * Mathf.PerlinNoise(0, Time.time));
        head_light.pointLightInnerAngle = current_setting.y * (1 + perturbation * Mathf.PerlinNoise(0, speed * Time.time));
        head_light.pointLightInnerRadius = current_setting.z * (1 + perturbation * Mathf.PerlinNoise(5, speed * Time.time));
        head_light.pointLightOuterRadius = current_setting.w * (1 + perturbation * Mathf.PerlinNoise(7, speed * Time.time));
    }
}
