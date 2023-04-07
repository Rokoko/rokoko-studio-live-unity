using UnityEngine;

namespace Rokoko.CommandAPI
{
    public enum BalancedNewtonPose
    {
        // tpose
        TPose = 0,

        // straight-arms-down
        StraightArmsDown = 1,
        
        // straight-arms-forward
        StraightArmsForward = 2,
    }

    public class CalibrateRequestData
    {
        [SerializeField] private string device_id; // the live input device hubName that the command should target   

        /// <summary>
        /// Count down in seconds before calibration is executed. -1 will use default setting.
        /// </summary>
        [SerializeField] private int countdown_delay = -1;  // countdown in seconds

        /// <summary>
        /// Skip Smartsuit Pro calibration.
        /// </summary>
        [SerializeField] private bool skip_suit = false; // should we skip suit from a processing (calibration)

        /// <summary>
        /// Skip Smartgloves calibration.
        /// </summary>
        [SerializeField] private bool skip_gloves = false; // should we skip gloves from a processing (calibration)

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private bool use_custom_pose = false;

        // useCustomPose is not Set, pose will be changed calibration 
        [SerializeField] private BalancedNewtonPose pose = BalancedNewtonPose.StraightArmsDown;

        // public members

        public string DeviceId { get => device_id; set => device_id = value; }
        public int CountDownDelay { get => countdown_delay; set => countdown_delay = value; }
        public bool SkipSuit { get => skip_suit; set => skip_suit = value; }
        public bool SkipGloves { get => skip_gloves; set => skip_gloves = value; }
        public bool UseCustomPose { get => use_custom_pose; set => use_custom_pose = value; } 
        public BalancedNewtonPose Pose { get => pose; set => pose = value; } 

        public override string ToString()
        {
            return DeviceId;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}