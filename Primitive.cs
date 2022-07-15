using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
namespace INFOGR2022Template
{
    internal class Primitive
    {
        //the material
        internal enum materials { diffuse, glossy, mirror };
        internal double reflectiveIndex;
        internal List <materials> material = new List<materials>();
        internal int id;
        //a vector 3 for the color. All float values must be between 0 and 1.
        internal Vector3 RGB;
        //bool for if this primitive has a texture
        internal bool hasTexture;
        internal Bitmap UVTexture;
        /// <summary>
        /// initialize
        /// </summary>
        /// <param name="RGB"></param>
        /// <param name="material"></param>
        /// <param name="reflectiveIndex"></param>
        public Primitive(Vector3 RGB, materials material = materials.diffuse , double reflectiveIndex = 1, string texture = null)
        {
            this.RGB = RGB;
            this.material.Add(material);
            this.reflectiveIndex = reflectiveIndex;
            this.hasTexture = hasTexture;
            if (texture != null)
            {
                hasTexture = true;
                UVTexture = new Bitmap(texture);
            }
        }

        /// <summary>
        /// this returns the normal, if given a distance. Every primitive must have this. 
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        internal virtual Vector3 ReturnNormal(Vector3 distance)
        {
            return new Vector3(0);
        }
        /// <summary>
        /// returns the color at a specific point
        /// is either something specific, or just the RGB colour
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal Vector3 GetColour(Vector3 point)
        {
            if (!hasTexture)
                return RGB;
            Vector2 UV = ReturnUVcord(point);
            Color color = UVTexture.GetPixel((int)(UV.X * UVTexture.Width), (int)(UV.Y * UVTexture.Height));
            return new Vector3(MathHelper.Clamp(color.R / 255f, 0, RGB.X),
                               MathHelper.Clamp(color.G / 255f, 0, RGB.Y),
                               MathHelper.Clamp(color.B / 255f, 0, RGB.Z));
        }
        /// <summary>
        /// retuns the Uv coordinates of the
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal virtual Vector2 ReturnUVcord(Vector3 point)
        {
            return new Vector2(0, 0);
        }
    }

    internal class Triangle : Primitive
    {
        Vector3 A, B, C;
        public Vector3 normal;
        /// <summary>
        /// init of the triangle. ABC must be clockwise, else the normal points the wrong direction.
        /// </summary>
        /// <param name="RGB"></param> colour of the triangle
        /// <param name="A"></param> point A of the triangle
        /// <param name="B"></param> point B of the triangle
        /// <param name="C"></param> point C of the triangle
        /// <param name="material"></param>
        /// <param name="reflectiveIndex"></param>
        public Triangle(Vector3 RGB, Vector3 A, Vector3 B, Vector3 C, materials material = materials.diffuse, string texture = null) : base(RGB, material, 1, texture)
        {
            this.A = A;
            this.B = B;
            this.C = C;
            normal = ReturnNormal(new Vector3(0, 0, 1));
        }

        /// <summary>
        /// returns if there is an intersection with the ray
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool IntersectTriangle(ref Raytracer.Ray ray)
        {
            //see if the ray is parallel
            float same = Vector3.Dot(ray.direction, normal);
            if (Math.Abs(same) <= 0.0001)
                return false;
            //calculate the, such that the ray intersects the plane the triangle is on
            float scalar = -(Vector3.Dot(normal, ray.position) - Vector3.Dot(normal, A)) / Vector3.Dot(normal, ray.direction);
            //check to see if the scalar isn't too close
            if (Math.Abs(scalar) <= 0.0001)
                return false;
            //set the scalar
            ray.scalar = scalar;
            Vector3 P = ray.position + (ray.direction * ray.scalar);
            //calculate if
            Vector3 crossA = Vector3.Cross(B - A, P - A);
            Vector3 crossB = Vector3.Cross(C - B, P - B);
            Vector3 crossC = Vector3.Cross(A - C, P - C);
            return Vector3.Dot(crossA, normal) >= 0f &&
                   Vector3.Dot(crossC, normal) >= 0f &&
                   Vector3.Dot(crossB, normal) >= 0f;
        }
        /// <summary>
        /// returns the normal of the triangle
        /// </summary>
        /// <returns></returns>
        internal override Vector3 ReturnNormal(Vector3 distance)
        {
            distance.Normalize();
            Vector3 norm = Vector3.Cross(B - A, C - A);
            norm.Normalize();
            //if the angle betwween the incoming ray and the normal > 90 degrees,then the ray is coming from the other side of the triangle
            if (Vector3.CalculateAngle(norm, distance) > MathHelper.DegreesToRadians(90))
                norm *= -1;
            return norm;
        }

        /// <summary>
        /// returns the UV cords of a triangle
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal override Vector2 ReturnUVcord(Vector3 point)
        {
            //value to divide the rest by
            float div = Vector3.Dot(Vector3.Cross(B - A, C - A), normal); 
            //calculate the barycentric values
            float alpha = Vector3.Dot(Vector3.Cross(C - B, point - B), normal) / div; 
            float beta = Vector3.Dot(Vector3.Cross(A - C, point - C), normal) / div;
            float gamma = 1 - alpha - beta;
            //get the UV coordinates, based on the UV values of A,B and C
            return new Vector2(1, 0) * alpha + new Vector2(0, 1) * beta + new Vector2(0, 0) * gamma;
        }
    }
}
