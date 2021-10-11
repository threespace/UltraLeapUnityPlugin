/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2020.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

namespace Leap.Unity
{

  /// <summary>
  /// Wraps various (but not all) "XR" calls with Unity 5.6-supporting "VR" calls
  // via #ifdefs.
  // </summary>
  public static class XRSupportUtil {

     // There is no support planned for Unity's XR plugin system on XR2 devices using the Unity SnapDragon XR SDK
     // The XR2 implementation therefore calls into the SVR plugin directly

     #if UNITY_ANDROID // In future we should target XR2 devices only with this. However, at the moment Android support is limited to XR2 devices only

     public static bool IsXREnabled() {
         return SvrManager.Instance.status.running && SvrManager.Instance.status.pose == 3;
     }

     public static bool IsXRDevicePresent() {
         return SvrManager.Instance.status.running;
     }

     public static bool IsUserPresent(bool defaultPresence = true) {
         return true; // TODO user presence can be queried with recent versions of the SVR SDK (>= v4.0.4) - a proximity detector is fitted to some devices.
     }

     public static Vector3 GetXRNodeCenterEyeLocalPosition() {
         if (XRSupportUtil.IsXREnabled()) {
             return SvrManager.Instance.head.transform.localPosition;
         } else {
             Debug.LogWarning("Cannot read XRNodeCenterEyeLocalPosition as XR is not enabled");
             return Vector3.zero;
         }
     }

     public static Quaternion GetXRNodeCenterEyeLocalRotation() {
         if (XRSupportUtil.IsXREnabled()) {
             return SvrManager.Instance.head.transform.localRotation;
         } else {
             Debug.LogWarning("Cannot read XRNodeCenterEyeLocalRotation as XR is not enabled");
             return Quaternion.identity;
         }
     }

     public static Vector3 GetXRNodeHeadLocalPosition() {
         if (XRSupportUtil.IsXREnabled())
         {
             return SvrManager.Instance.head.localPosition;
         }
         else
         {
             Debug.LogWarning("Cannot read XRNodeHeadLocalPosition as XR is not enabled");
             return Vector3.zero;
         }
     }

     public static Quaternion GetXRNodeHeadLocalRotation() {
         if (XRSupportUtil.IsXREnabled())
         {
             return SvrManager.Instance.head.transform.localRotation;
         }
         else
         {
             Debug.LogWarning("Cannot read XRNodeHeadLocalRotation as XR is not enabled");
             return Quaternion.identity;
         }
     }

     public static Vector3 GetXRNodeLocalPosition(int node) {
          Debug.LogWarning("Not implemented yet - waiting on max");
          return Vector3.zero;
     }

     public static Quaternion GetXRNodeLocalRotation(int node) {
          Debug.LogWarning("Not implemented yet - waiting on max");
          return Quaternion.identity;
     }

     public static void Recenter() {
         SvrInput.Instance.HandleRecenter();
     }

     public static float GetGPUTime() {
          Debug.LogWarning("Not implemented for android");
          return 0;
     }

     public static string GetLoadedDeviceName() {
         return "XR2_Device";
     }

     public static bool IsRoomScale() {
         return true;
     }

     public static bool IsLargePlayspace() {
         // The current SVR support for reading the playspace dimensions is crude. Likely to be improved on by 3rd parties
         // By default, we should consider the current playspace as 'large'
         return true;
     }

     // The following methods / fields are part of the XRSupportUtil 'interface' but are not currently used. They have not been implemented for the XR2/Android
     // public static float GetGPUTime() {
     // }

    #else

    #if UNITY_2019_2_OR_NEWER
    private static System.Collections.Generic.List<XRNodeState> nodeStates = 
      new System.Collections.Generic.List<XRNodeState>();
    #endif

    public static bool IsXREnabled() {
      #if UNITY_2017_2_OR_NEWER
      return XRSettings.enabled;
      #else
      return VRSettings.enabled;
      #endif
    }

    public static bool IsXRDevicePresent() {
      #if UNITY_2020_1_OR_NEWER
      return XRSettings.isDeviceActive;
      #elif UNITY_2017_2_OR_NEWER
      return XRDevice.isPresent;
      #else
      return VRDevice.isPresent;
      #endif
    }

    static bool outputPresenceWarning = false;
    public static bool IsUserPresent(bool defaultPresence = true) {
      #if UNITY_2019_3_OR_NEWER
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
        if (devices.Count == 0 && !outputPresenceWarning) {
          Debug.LogWarning("No head-mounted devices found. Possibly no HMD is available to the XR system.");
          outputPresenceWarning = true;
        }
        if (devices.Count != 0) {
          var device = devices[0];
          if (device.TryGetFeatureValue(CommonUsages.userPresence, out var userPresent)) {
            return userPresent;
          }
        }
      #elif UNITY_2017_2_OR_NEWER
        var userPresence = XRDevice.userPresence;
        if (userPresence == UserPresenceState.Present) {
          return true;
        } else if (!outputPresenceWarning && userPresence == UserPresenceState.Unsupported) {
          Debug.LogWarning("XR UserPresenceState unsupported (XR support is probably disabled).");
          outputPresenceWarning = true;
        }
      #else
        if (!outputPresenceWarning){
          Debug.LogWarning("XR UserPresenceState is only supported in 2017.2 and newer.");
          outputPresenceWarning = true;
        }
      #endif
      return defaultPresence;
    }

    public static Vector3 GetXRNodeCenterEyeLocalPosition() {
      #if UNITY_2019_2_OR_NEWER
      InputTracking.GetNodeStates(nodeStates);
      Vector3 position;
      foreach(XRNodeState state in nodeStates) {
        if(state.nodeType == XRNode.CenterEye &&
           state.TryGetPosition(out position))
        { return position; }
      }
      return Vector3.zero;
      #elif UNITY_2017_2_OR_NEWER
      return InputTracking.GetLocalPosition(XRNode.CenterEye);
      #else
      return InputTracking.GetLocalPosition(VRNode.CenterEye);
      #endif
    }

    public static Quaternion GetXRNodeCenterEyeLocalRotation() {
      #if UNITY_2019_2_OR_NEWER
      InputTracking.GetNodeStates(nodeStates);
      Quaternion rotation;
      foreach (XRNodeState state in nodeStates) {
        if (state.nodeType == XRNode.CenterEye &&
            state.TryGetRotation(out rotation))
        { return rotation; }
      }
      return Quaternion.identity;
      #elif UNITY_2017_2_OR_NEWER
      return InputTracking.GetLocalRotation(XRNode.CenterEye);
      #else
      return InputTracking.GetLocalRotation(VRNode.CenterEye);
      #endif
    }

    public static Vector3 GetXRNodeHeadLocalPosition() {
      #if UNITY_2019_2_OR_NEWER
      InputTracking.GetNodeStates(nodeStates);
      Vector3 position;
      foreach(XRNodeState state in nodeStates) {
        if(state.nodeType == XRNode.Head &&
           state.TryGetPosition(out position))
        { return position; }
      }
      return Vector3.zero;
      #elif UNITY_2017_2_OR_NEWER
      return InputTracking.GetLocalPosition(XRNode.Head);
      #else
      return InputTracking.GetLocalPosition(VRNode.Head);
      #endif
    }

    public static Quaternion GetXRNodeHeadLocalRotation() {
      #if UNITY_2019_2_OR_NEWER
      InputTracking.GetNodeStates(nodeStates);
      Quaternion rotation;
      foreach (XRNodeState state in nodeStates) {
        if (state.nodeType == XRNode.Head &&
            state.TryGetRotation(out rotation))
        { return rotation; }
      }
      return Quaternion.identity;
      #elif UNITY_2017_2_OR_NEWER
      return InputTracking.GetLocalRotation(XRNode.Head);
      #else
      return InputTracking.GetLocalRotation(VRNode.Head);
      #endif
    }

    public static Vector3 GetXRNodeLocalPosition(int node) {
      #if UNITY_2019_2_OR_NEWER
      InputTracking.GetNodeStates(nodeStates);
      Vector3 position;
      foreach(XRNodeState state in nodeStates) {
        if(state.nodeType == (XRNode)node &&
           state.TryGetPosition(out position))
        { return position; }
      }
      return Vector3.zero;
      #elif UNITY_2017_2_OR_NEWER
      return InputTracking.GetLocalPosition((XRNode)node);
      #else
      return InputTracking.GetLocalPosition((VRNode)node);
      #endif
    }

    public static Quaternion GetXRNodeLocalRotation(int node) {
      #if UNITY_2019_2_OR_NEWER
      InputTracking.GetNodeStates(nodeStates);
      Quaternion rotation;
      foreach (XRNodeState state in nodeStates) {
        if (state.nodeType == (XRNode)node &&
            state.TryGetRotation(out rotation))
        { return rotation; }
      }
      return Quaternion.identity;
      #elif UNITY_2017_2_OR_NEWER
      return InputTracking.GetLocalRotation((XRNode)node);
      #else
      return InputTracking.GetLocalRotation((VRNode)node);
      #endif
    }

    public static void Recenter() {
      #if UNITY_2019_3_OR_NEWER
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
        if (devices.Count == 0) return;
        var hmdDevice = devices[0];
        #if !UNITY_2020_1_OR_NEWER
        if(hmdDevice.subsystem != null) {
        #endif
          hmdDevice.subsystem.TryRecenter();
        #if !UNITY_2020_1_OR_NEWER
        }else{
          #pragma warning disable 0618
          InputTracking.Recenter();
          #pragma warning restore 0618
        }
        #endif
      #else
        InputTracking.Recenter();
      #endif
    }

    public static string GetLoadedDeviceName() {
      #if UNITY_2017_2_OR_NEWER
      return XRSettings.loadedDeviceName;
      #else
      return VRSettings.loadedDeviceName;
      #endif
    }

    /// <summary> Returns whether there's a floor available. </summary>
    public static bool IsRoomScale() {
      #if UNITY_2019_3_OR_NEWER
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
        if (devices.Count == 0) return false;
        var hmdDevice = devices[0];
        #if !UNITY_2020_1_OR_NEWER
        if(hmdDevice.subsystem != null) {
        #endif
          return hmdDevice.subsystem.GetTrackingOriginMode().HasFlag(TrackingOriginModeFlags.Floor);
        #if !UNITY_2020_1_OR_NEWER
        }else{
          #pragma warning disable 0618
          return XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale;
          #pragma warning restore 0618
        }
        #endif
      #elif UNITY_2017_2_OR_NEWER
        return XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale;
      #else
        return VRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale;
      #endif
    }

    #if UNITY_2020_1_OR_NEWER
    static List<Vector3> _boundaryPoints = new List<Vector3>();
    #endif
    /// <summary> Returns whether the playspace is larger than 1m on its shortest side. </summary>
    public static bool IsLargePlayspace() {
      #if UNITY_2020_1_OR_NEWER // Oculus reports a floor centered space now...
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
        if (devices.Count == 0) return false;
        var hmdDevice = devices[0];
        hmdDevice.subsystem.TryGetBoundaryPoints(_boundaryPoints);
        Bounds playspaceSize = new Bounds();
        foreach(Vector3 boundaryPoint in _boundaryPoints) { playspaceSize.Encapsulate(boundaryPoint); }
        return playspaceSize.size.magnitude > 1f; // Playspace is greater than 1m on its shortest axis
      #else
        return IsRoomScale();
      #endif
    }

    public static float GetGPUTime() {
      float gpuTime = 0f;
      #if UNITY_5_6_OR_NEWER
      #if UNITY_2017_2_OR_NEWER
      UnityEngine.XR.XRStats.TryGetGPUTimeLastFrame(out gpuTime);
      #else
      UnityEngine.VR.VRStats.TryGetGPUTimeLastFrame(out gpuTime);
      #endif
      #else
      gpuTime = UnityEngine.VR.VRStats.gpuTimeLastFrame;
      #endif
      return gpuTime;
    }

    #endif
    }
}
