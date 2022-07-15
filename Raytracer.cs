using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Template;

namespace INFOGR2022Template
{
    internal class Raytracer
    {
        Scene scene;
        Camera camera;
        //the ray that will be set through the the camera origin and the screen plane
        Ray primaryRay = new Ray();
        //the intersection
        Intersection intersection = new Intersection();
        //upleft corner
        Vector3 upLeft;
        //upRight
        Vector3 upRight;
        //downRight corner
        Vector3 downLeft;
        public Raytracer(Scene scene, Camera camera)
        {
            this.scene = scene;
            UpdateCam(camera);
        }
        internal void Render()
        {
            //the width of the plane
            Vector3 horizon = upLeft - upRight;
            //the height of the plane
            Vector3 vertical = downLeft - upLeft;
            //set the pos of the camera
            primaryRay.position = camera.cameraPos;

            for (int y = 0; y < OpenTKApp.app.screen.height; y++)
            {
                for (int x = 0; x < OpenTKApp.app.screen.width; x++)
                {
                    //the RGB vec3 of that pixel on the screen
                    Vector3 RGB = new Vector3(0);
                    //reset the primary ray and set the direction
                    Intersection nearest = new Intersection();
                    nearest.ray.scalar = float.MaxValue;
                    primaryRay.scalar = 0;
                    primaryRay.direction = upLeft - (x / (float)OpenTKApp.app.screen.width) * horizon + (y / (float)OpenTKApp.app.screen.height) * vertical;
                    foreach (Primitive P in scene.objects)
                    {
                        if (Intersect(P, ref primaryRay))
                        {
                            //checks to see if the other primitive is closer to the screen
                            if (primaryRay.scalar < nearest.ray.scalar)
                            {
                                nearest.ray = primaryRay;
                                nearest.lastHit = P;
                            }
                        }
                    }

                    if (nearest.lastHit != null)
                        RGB = returnColorLight(nearest.lastHit, nearest.ray);
                    OpenTKApp.app.screen.Plot(x, y, MixColor(
                        (int)(MathHelper.Clamp(RGB.X, 0f, 1f) * 255),
                        (int)(MathHelper.Clamp(RGB.Y, 0f, 1f) * 255),
                        (int)(MathHelper.Clamp(RGB.Z, 0f, 1f) * 255)));
                }
            }
        }
        Vector3 returnColorLight(Primitive p, Ray calcRay)
        {
            //create a shadow ray and set its position

            Ray shadowRay = new Ray();
            shadowRay.position = calcRay.position + calcRay.direction * calcRay.scalar;
            Vector3 color = p.GetColour(calcRay.position + calcRay.direction * calcRay.scalar);
            //then for every light
            foreach (Light l in scene.lights)
            {
                //reset the scalar and set the normal direction
                shadowRay.scalar = 0;
                shadowRay.direction = -l.position + shadowRay.position;
                //see if the light can be hit 
                if (!l.IsTrue(shadowRay.position + shadowRay.direction))
                    continue;

                bool hitAny = false;
                foreach (Primitive toCheck in scene.objects)
                {
                    hitAny = Intersect(toCheck, ref shadowRay);
                    if (hitAny)
                        break;
                }
                //if it doesn't hit any, set the color
                if (!hitAny)
                {
                    foreach (Primitive.materials m in p.material)
                    {
                        calcRay.RGB += l.returnColor(p.ReturnNormal(shadowRay.position),
                            shadowRay.direction,
                            camera.lookAtDirection,
                            color,
                            m);
                    }
                }
            }
            return calcRay.RGB + new Vector3(0.07f);
        }

        bool Intersect(Primitive P, ref Ray ray)
        {
            if (P is Triangle)
                return IntersectTriangle((Triangle)P, ref ray);
            return false;
        }

        bool IntersectTriangle(Triangle P, ref Ray ray)
        {
            if (P.IntersectTriangle(ref ray))
            {
                intersection.ray = ray;
                intersection.lastHit = P;
                return true;
            }
            return false;
        }

        public void UpdateCam(Camera camera)
        {
            this.camera = camera;
            primaryRay.position = camera.cameraPos;
            Vector3 center = camera.cameraPos + camera.lookAtDirection;
            Vector3 rightDirection = Vector3.Cross(camera.upDirection, camera.lookAtDirection);
            //calculating the edge points of the plane
            upLeft = center + camera.upDirection - rightDirection;
            upRight = center + camera.upDirection + rightDirection;
            downLeft = center - camera.upDirection - rightDirection;
        }
        internal struct Ray
        {
            internal Vector3 RGB;
            internal Vector3 position;
            internal float scalar;
            internal Vector3 direction;
            internal int amountOfRecursion;
        }
        int MixColor(int red, int green, int blue)
        {
            return (red << 16) + (green << 8) + blue;
        }
    }
}
