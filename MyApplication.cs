using INFOGR2022Template;
using OpenTK;
using System;
namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Raytracer raytracer;
        internal Camera camera;
        internal Scene scene;
        Vector3 cameraPos, upDirection, lookAtDirection;
        int gloss = 8;
        float PI = (float)Math.PI;
        // initialize
        public void Init()
        {
            cameraPos = new Vector3(0.6f, 0.4f, 1f);
            lookAtDirection = new Vector3(0, 0, 1);
            upDirection = new Vector3(0, 1, 0);
            camera = new Camera(cameraPos, lookAtDirection, upDirection);
            scene = new Scene();

            Triangle t = new Triangle(new Vector3(1, 1, 1),
                new Vector3(0, 0, 20), new Vector3(10, 0, 20), new Vector3(7, 8, 18),
                Primitive.materials.glossy, "../../assets/wood.jpg");
            t.material.Add(Primitive.materials.diffuse);
            Triangle t2 = new Triangle(new Vector3(1, 0, 1),
                new Vector3(0.4f, 0.3f, 1.1f), new Vector3(0.6f, 0.3f, 1.1f), new Vector3(0.5f, 0.6f, 1.1f));
            scene.objects.Add(t);
            scene.objects.Add(t2);
            scene.lights.Add(new Light(new Vector3(2, 2, 2f), new Vector3(1, 1, 1), 10f, gloss));
            scene.lights.Add(new Spotlight(new Vector3(0.5f, 0.5f, 1f), new Vector3(1, 1, 1), 30f, gloss,
                new Vector3(0.3302435f, 0.3807125f, 0.8637114f), 15f));

            raytracer = new Raytracer(scene, camera);
        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0x000000);
            raytracer.Render();
        }
        public void Move(int direction)
        {
            switch (direction)
            {
                case (0): //Right
                    cameraPos += Vector3.Cross(upDirection, lookAtDirection)/10f;
                    break;

                case (1): //Left
                    cameraPos -= Vector3.Cross(upDirection, lookAtDirection) / 10f;
                    break;

                case (2): //up
                    cameraPos += upDirection / 10f;
                    break;

                case (3): //down
                    cameraPos -= upDirection / 10f;
                    break;

                case (4): //forward
                    cameraPos += lookAtDirection / 10f;
                    break;

                case (5): //backward
                    cameraPos -= lookAtDirection / 10f;
                    break;

                case (6): //rotate the lookAtDirection around the updirection
                    lookAtDirection = Vector3.TransformPerspective(lookAtDirection, Matrix4.CreateFromAxisAngle(upDirection, PI / 16));
                    break;

                case (7):// rotate the updirection around the lookatdirection
                    upDirection = Vector3.TransformPerspective(upDirection, Matrix4.CreateFromAxisAngle(lookAtDirection, PI / 16));
                    break;
                case (8): //rotate forward
                    Vector3 side = Vector3.Cross(upDirection, lookAtDirection);
                    upDirection = Vector3.TransformPerspective(upDirection, Matrix4.CreateFromAxisAngle(side, PI / 16));
                    lookAtDirection = Vector3.TransformPerspective(lookAtDirection, Matrix4.CreateFromAxisAngle(side, PI / 16));
                    break;

            }
            upDirection.Normalize();
            lookAtDirection.Normalize();
            camera = new Camera(cameraPos, lookAtDirection, upDirection);
            raytracer.UpdateCam(camera);
        }
    }
}