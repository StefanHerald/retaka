using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace INFOGR2022Template
{
    internal class Light : Primitive
    {
        //position of the light
        internal Vector3 position;
        internal float watt;
        int gloss = 1;
        public Light(Vector3 position, Vector3 RGB, float watt, int gloss) : base(RGB)
        {
            this.gloss = gloss;
            this.position = position;
            this.watt = watt;
        }

        internal Vector3 returnColor(Vector3 normal, Vector3 lightDirection, Vector3 lookAtDirection, Vector3 color, Primitive.materials material)
        {
            Vector3 reflected = new Vector3();
            switch (material)
            {
                case Primitive.materials.diffuse:
                    reflected = (1f / lightDirection.LengthSquared) * watt * RGB * color * Math.Max(0, Vector3.Dot(normal, lightDirection));
                    break;
                case Primitive.materials.glossy:
                    Vector3 R = lightDirection - 2 * (lightDirection * normal) * normal;
                    reflected = (1f / lightDirection.LengthSquared) * watt * RGB * color * (float)Math.Pow(Math.Max(0, Vector3.Dot(R, lookAtDirection)), gloss);
                    break;
            }
            return reflected;
        }

        /// <summary>
        /// checks if the light can be hit by the given ray
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        internal virtual bool IsTrue(Vector3 ray)
        {
            return true;
        }
    }
    internal class Spotlight : Light
    {
        Vector3 lightDirection;
        float angle;
        /// <summary>
        /// creates a spotlight, given a light direction and a maximum angle (in degrees)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="RGB"></param>
        /// <param name="watt"></param>
        /// <param name="gloss"></param>
        /// <param name="lightDirection"></param> the direction the light goes
        /// <param name="angle"></param> maximum angle between the incoming ray and the lightdirection
        public Spotlight(Vector3 pos, Vector3 RGB, float watt, int gloss, Vector3 lightDirection, float angle) : base(pos, RGB, watt, gloss)
        {
            this.lightDirection = lightDirection;
            this.angle = MathHelper.DegreesToRadians(angle);
        }

        /// <summary>
        /// checks if the angle between the incoming ray and the light direction is less than the max angle
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        internal override bool IsTrue(Vector3 ray)
        {
            return Vector3.CalculateAngle(lightDirection, ray) <= angle;
        }
    }
}
