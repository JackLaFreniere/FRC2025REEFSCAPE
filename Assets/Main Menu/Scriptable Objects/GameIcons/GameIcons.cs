using UnityEngine;

[CreateAssetMenu(fileName = "GameIcons", menuName = "Scriptable Objects/GameIcons")]
public class GameIcons : ScriptableObject
{
    public string iconName;
    public Texture2D iconTexture;
}
