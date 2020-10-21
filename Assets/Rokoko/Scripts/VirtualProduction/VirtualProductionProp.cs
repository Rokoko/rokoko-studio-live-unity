using System.Linq;
using UnityEngine;

namespace Rokoko.VirtualProduction
{
    /// <inheritdoc />
    /// <summary>
    /// Makes the game object follow the position and rotation of a prop. 
    /// </summary>
    public class VirtualProductionProp : MonoBehaviour
    {
        public string propName;

        private Transform _transform;

        private void Start() => _transform = transform;

        // Update is called once per frame
        private void Update()
        {
            var prop = VirtualProductionReceiver.Instance.VirtualProductionData.props.FirstOrDefault(data =>
                data.name == propName);
            if (prop == null || prop.name != propName) return;

            _transform.position = prop.position;
            _transform.rotation = prop.rotation;
        }
    }
}