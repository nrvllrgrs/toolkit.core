using UnityEngine;

namespace ToolkitEngine
{
    public class LookAtCamera : MonoBehaviour
    {
        #region Enumerators

        public enum LookStyle
        {
            Default,
			Axis,
			Parallel,
        }

        #endregion

        #region Fields

        public LookStyle lookStyle;
        public Axis axis = Axis.Up;
        public bool reverse;

        #endregion

        #region Methods

        private void Update()
        {
            if (lookStyle == LookStyle.Parallel)
            {
                if (!reverse)
                {
                    transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
                }
            }
            else
            {
                var position = Camera.main.transform.position;
                if (lookStyle == LookStyle.Axis)
                {
                    switch (axis)
                    {
                        case Axis.Left:
                        case Axis.Right:
                            position.x = transform.position.x;
                            break;

                        case Axis.Up:
                        case Axis.Down:
                            position.y = transform.position.y;
                            break;

                        case Axis.Forward:
                        case Axis.Back:
                            position.z = transform.position.z;
                            break;
                    }
                }

				if (!reverse)
				{
					transform.LookAt(position, Vector3.up);
				}
				else
				{
					transform.LookAt(2f * transform.position - position, Vector3.up);
				}
			}
        }

        #endregion
    }
}