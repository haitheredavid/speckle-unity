using System;
using System.Collections.Generic;
using Objects.Geometry;
using Speckle.ConnectorUnity.Core.ScriptableConverter;
using Unity.Collections;
using UnityEngine;

namespace Speckle.ConnectorUnity
{

    [CreateAssetMenu(fileName = nameof(PointCloudToParticleSystem), menuName = SpeckleUnity.Categories.CONVERTERS + "Create Point Cloud Converter")]
    public class PointCloudToParticleSystem : ComponentConverter<Pointcloud, ParticleSystem>{
    
        protected override void Serialize(Pointcloud obj, ParticleSystem target)
        {
            if (obj == null || !obj.points.Valid())
            {
                Debug.Log($"Error with converting {obj} to particle system");
                return;
            }


            var particles = new List<ParticleSystem.Particle>();

            for (int index = 2; index < obj.points.Count; index += 3)
            {
                particles.Add(
                    new ParticleSystem.Particle
                    {
                        position = new Vector3(
                            (float)obj.points[index - 2],
                            (float)obj.points[index - 1],
                            (float)obj.points[index])
                    });
            }

            target.SetParticles(particles.ToArray());
        }

        protected override Pointcloud Deserialize(ParticleSystem obj)
        {
            var native = new NativeArray<ParticleSystem.Particle>();
            obj.GetParticles(native);
            var points = new List<double>();

            for (int i = 0; i < native.Length; i += 3)
            {
                // check on scale? 
                ParticleSystem.Particle particle = native[i];
                points.Add(particle.position.x);
                points.Add(particle.position.y);
                points.Add(particle.position.z);
            }

            return new Pointcloud(points);
        }
    }

}
