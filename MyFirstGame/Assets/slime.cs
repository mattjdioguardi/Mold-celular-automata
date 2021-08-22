using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct Particle {
    public Vector2 position;
    public float angle;

}

public class slime : MonoBehaviour
{

    public ComputeShader computeShader;
    public ComputeShader computeShader2;


    public RenderTexture renderTexture;
    
    [Range(16, 10000000)]
    public int count = 1000000;
    [Range(0, 1000)]
    public float speed = 1.00F;
    [Range(8, 3480)]
    public int width = 2560;
    [Range(8, 2160)]
    public int height = 1440;
    [Range(0, 100)]
    public float sensor_offset = 9.0F;
    [Range(0, 20)]
    public float sensor_width = 2.0F;
    [Range(0, 360)]
    public float sensor_angle = 45F;
    [Range(0, 360)]
    public float turn = 35F;
    [Range(0, 200)]
    public float diffusion = 10F;
    [Range(0, 100)]
    public float decay = .2F;

    private Particle[] data;


    public void CreateParticles()
    {
        //objects = new List<GameObject>();

        data = new Particle[count];

        for (int x = 0; x < count; x++)
        {
            Particle particle = new Particle();
            //particle.position = new Vector2(renderTexture.width / 2, renderTexture.height/ 2);
            particle.position = new Vector2(Random.Range(0, width), Random.Range(0, height));
            particle.angle = Random.Range(0, 360);
            data[x] = particle;
        }
    }


    void Start()
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(width, height, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
        CreateParticles();

    }


    public void GPU() 
    {
        int particle_size = sizeof(float) * 3;
        ComputeBuffer particleBuffer = new ComputeBuffer(data.Length, particle_size);
        particleBuffer.SetData(data);


        computeShader.SetBuffer(0, "particles", particleBuffer);
        computeShader.SetTexture(0, "TrailMap", renderTexture);
        computeShader.SetInt("width", renderTexture.width);
        computeShader.SetInt("height", renderTexture.height);
        computeShader.SetInt("count", count);
        computeShader.SetFloat("sensor_offset", sensor_offset);
        computeShader.SetFloat("sensor_width", sensor_width);
        computeShader.SetFloat("sensor_angle", sensor_angle);
        computeShader.SetFloat("speed", speed);
        computeShader.SetFloat("turn", turn);



        computeShader.Dispatch(0, data.Length/1024, 1,1);

        particleBuffer.GetData(data);

        particleBuffer.Dispose();
    }


    public void trails() {
        computeShader2.SetTexture(0, "TrailMap", renderTexture);
        computeShader2.SetFloat("time", Time.deltaTime);
        computeShader2.SetFloat("diffusion", diffusion);
        computeShader2.SetFloat("decay", decay);
        computeShader2.SetInt("width", renderTexture.width);
        computeShader2.SetInt("height", renderTexture.height);

        computeShader2.Dispatch(0, renderTexture.width + 8/ 8, renderTexture.height + 8 / 8, 1);

    }


    public void test() {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(300, 175, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
        computeShader.SetTexture(0, "Result", renderTexture);
        //computeShader.SetFloat("Resolution", renderTexture.width);

        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);


    }



    private void OnRenderImage(RenderTexture src, RenderTexture dest) {

        
        GPU();
        trails();

        Graphics.Blit(renderTexture, dest);

    }
}
