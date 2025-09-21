using System;
using UnityEngine;

public class MukwonagoBotPresets : MonoBehaviour
{
    public static CoralReefLevel coralReefLevel = CoralReefLevel.L4;
    public static AlgaeReefLevel algaeReefLevel = AlgaeReefLevel.High;

    public static event Action OnReefLevelChanged;

    public static void CycleReefLevel()
    {
        coralReefLevel = GetPreviousEnumValue(coralReefLevel);
        algaeReefLevel = GetPreviousEnumValue(algaeReefLevel);
        OnReefLevelChanged?.Invoke();
    }

    public static T GetPreviousEnumValue<T>(T current) where T : Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        int currentIndex = Array.IndexOf(values, current);
        int prevIndex = (currentIndex - 1 + values.Length) % values.Length;
        return values[prevIndex];
    }
}