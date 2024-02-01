using UnityEngine;

// https://discussions.unity.com/t/how-to-make-a-readonly-property-in-inspector/75448/5
/// <summary> An attribute that makes a public field visible in the inspector, but not editable</summary>
public class ReadOnlyAttribute : PropertyAttribute
{
    
}
