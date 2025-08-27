using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement rootElement;

    private Button coralButton;
    private Button algaeButton;

#pragma warning disable IDE0044
    private Button nextCoralButton;
    private Button nextAlgaeButton;
#pragma warning restore IDE0044

    private StyleColor unselectedColor;
    private StyleFloat unselectedBorderWidth;

    private StyleColor nextColor = new (Color.yellow);
    private StyleColor selectedColor = new (Color.green);
    private StyleFloat selectedBorderWidth = new (5f);

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        rootElement = uiDocument.rootVisualElement;

        //Obtain instances of the current buttons and the next buttons
        GetAllButtons();

        //Store the unselected button styles
        unselectedColor = coralButton.style.borderTopColor.value;
        unselectedBorderWidth = coralButton.style.borderTopWidth.value;

        //Highlight the current and next buttons
        HighlightButtons(coralButton, algaeButton, selectedColor);
        HighlightButtons(nextCoralButton, nextAlgaeButton, nextColor);
    }

    private void OnEnable()
    {
        //Subscribes to the static event in MukwonagoBotPresets to update the buttons when the reef level changes
        MukwonagoBotPresets.OnReefLevelChanged += UpdateButtons;
    }

    private void OnDisable()
    {
        //Unsubscribes from the static event in MukwonagoBotPresets to prevent memory leaks
        MukwonagoBotPresets.OnReefLevelChanged -= UpdateButtons;
    }

    /// <summary>
    /// Updates the state of the buttons by unhighlighting the current selection, retrieving all buttons, and
    /// highlighting the new selection.
    /// </summary>
    /// <remarks>This method ensures that the button states are refreshed by first clearing any existing
    /// highlights,  retrieving the current set of buttons, and then applying highlights to the appropriate
    /// buttons.</remarks>
    public void UpdateButtons()
    {
        UnhighlightButtons(coralButton, algaeButton);
        GetAllButtons();
        HighlightNewButtons();
    }

    /// <summary>
    /// Retrieves a button element by its name or identifier.
    /// </summary>
    /// <param name="str">The name or identifier of the button to retrieve. Cannot be null or empty.</param>
    /// <returns>The <see cref="Button"/> element that matches the specified name or identifier, or <see langword="null"/> if no
    /// matching button is found.</returns>
    private Button GetButton(string str)
    {
        return rootElement.Q<Button>(str);
    }

    /// <summary>
    /// Retrieves and initializes all button references required for the application.
    /// </summary>
    /// <remarks>This method assigns specific buttons based on predefined presets and invokes additional logic
    /// to retrieve subsequent button references. It is intended to ensure that all necessary buttons      are properly
    /// initialized before use.</remarks>
    private void GetAllButtons()
    {
        coralButton = GetButton(MukwonagoBotPresets.coralReefLevel.ToString() + "Coral");
        algaeButton = GetButton(MukwonagoBotPresets.algaeReefLevel.ToString());

        GetNextButtons();
    }

    /// <summary>
    /// Updates the references to the next buttons in the sequence for the coral and algae levels.
    /// </summary>
    /// <remarks>This method retrieves the buttons corresponding to the previous enum values of the current
    /// coral and algae reef levels and assigns them to the respective fields. The button names are constructed based on
    /// the enum values.</remarks>
    private void GetNextButtons()
    {
        nextCoralButton = GetButton(MukwonagoBotPresets.GetPreviousEnumValue(MukwonagoBotPresets.coralReefLevel).ToString() + "Coral");
        nextAlgaeButton = GetButton(MukwonagoBotPresets.GetPreviousEnumValue(MukwonagoBotPresets.algaeReefLevel).ToString());
    }

    /// <summary>
    /// Highlights the specified buttons with the appropriate colors to indicate their current state.
    /// </summary>
    /// <remarks>This method applies visual highlighting to a set of buttons by delegating to the
    /// <c>HighlightButtons</c> method. It is used to update the appearance of buttons representing different states or
    /// categories.</remarks>
    private void HighlightNewButtons()
    {
        HighlightButtons(coralButton, algaeButton, selectedColor);
        HighlightButtons(nextCoralButton, nextAlgaeButton, nextColor);
    }

    /// <summary>
    /// Highlights the specified buttons by applying a border with the given color and width.
    /// </summary>
    /// <remarks>This method updates the visual appearance of the provided buttons by setting their border to
    /// the specified color and a predefined width.</remarks>
    /// <param name="coral">The first button to highlight.</param>
    /// <param name="algae">The second button to highlight.</param>
    /// <param name="color">The color to apply to the border of the buttons.</param>
    private void HighlightButtons(Button coral, Button algae, StyleColor color)
    {
        SetButtonBorder(coral, color, selectedBorderWidth);
        SetButtonBorder(algae, color, selectedBorderWidth);
    }

    /// <summary>
    /// Resets the visual state of the specified buttons to indicate they are unselected.
    /// </summary>
    /// <remarks>This method updates the border color and width of the provided buttons to reflect an
    /// unselected state.</remarks>
    /// <param name="coral">The first button to unhighlight.</param>
    /// <param name="algae">The second button to unhighlight.</param>
    private void UnhighlightButtons(Button coral, Button algae)
    {
        SetButtonBorder(coral, unselectedColor, unselectedBorderWidth);
        SetButtonBorder(algae, unselectedColor, unselectedBorderWidth);
    }

    /// <summary>
    /// Configures the border color and width for the specified button.
    /// </summary>
    /// <param name="button">The button whose border will be styled. Cannot be <see langword="null"/>.</param>
    /// <param name="color">The color to apply to all sides of the button's border.</param>
    /// <param name="width">The width to apply to all sides of the button's border.</param>
    private void SetButtonBorder(Button button, StyleColor color, StyleFloat width)
    {
        button.style.borderTopColor = color;
        button.style.borderBottomColor = color;
        button.style.borderLeftColor = color;
        button.style.borderRightColor = color;

        button.style.borderTopWidth = width;
        button.style.borderBottomWidth = width;
        button.style.borderLeftWidth = width;
        button.style.borderRightWidth = width;
    }
}