using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace INFOGR2022Template
{
    internal class Camera
    {
        //this stores the position relative to the updirection of the camera
        internal Vector3 cameraPos;
        //this vector stores the direction the camera looks at
        internal Vector3 lookAtDirection;
        //the up direction of the camera for the plane
        internal Vector3 upDirection;
        // the Field Of View of the camera
        internal float FOV;

        public Camera(Vector3 position, Vector3 lookAtDirection, Vector3 upDirection, float alpha = 120)
        {
            this.cameraPos = position;
            this.lookAtDirection = lookAtDirection;
            lookAtDirection.Normalize();
            this.upDirection = upDirection;
            upDirection.Normalize();
            //calculate the FOV scale for the lookatDirection. both the length of the rightDirection and the lookatdirection are normalized, so we simplified.
            FOV = 1 / (float)Math.Tan(MathHelper.DegreesToRadians(alpha / 2));
        }
    }
}
