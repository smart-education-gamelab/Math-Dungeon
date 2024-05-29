using UnityEngine;

public class ParticleOnLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public ParticleSystem particleSystem;
    public int numberOfParticles;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particles = new ParticleSystem.Particle[numberOfParticles];
        SpawnParticlesAlongLine();
    }

    void SpawnParticlesAlongLine()
    {
        float lineLength = lineRenderer.positionCount;
        for (int i = 0; i < numberOfParticles; i++)
        {
            float t = i / (float)numberOfParticles;
            Vector3 position = lineRenderer.GetPosition(Mathf.FloorToInt(t * lineLength));
            particles[i].position = position;
            particles[i].startColor = Color.white; // Or set your desired color
            particles[i].startSize = 0.1f; // Or set your desired size
        }

        particleSystem.SetParticles(particles, particles.Length);
    }
}