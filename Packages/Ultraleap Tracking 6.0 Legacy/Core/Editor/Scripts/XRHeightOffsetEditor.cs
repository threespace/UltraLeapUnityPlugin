/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2023.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Leap.Unity
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XRHeightOffset))]

    public class XRHeightOffsetEditor : CustomEditorBase<XRHeightOffset>
    {

        protected override void OnEnable()
        {
            base.OnEnable();

            specifyConditionalDrawing(conditionalName: "recenterOnKey",
                                      dependantProperties: "recenterKey");
        }

    }

}