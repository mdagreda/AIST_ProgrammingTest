using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

/// <summary>
/// This class handles displaying the left or right text depending on user input
/// </summary>
public class LeftRightGuiController : MonoBehaviour
{
    /// <summary>
    /// The Gui panel to display the left and right text and left and right buttons
    /// </summary>
    public GameObject guiPanel;

    /// <summary>
    /// The text to display left and right depending on the input
    /// </summary>
    public TMP_Text LeftRightText;

    /// <summary>
    /// The left button to press to display left text
    /// </summary>
    public Button leftButton;

    /// <summary>
    /// The right button to press to display right text
    /// </summary>
    public Button rightButton;

    private bool guiActivated = false;

    /// <summary>
    /// The left controller action based input
    /// </summary>
    [SerializeField]
    private ActionBasedController leftController;

    /// <summary>
    /// The right controller action based input
    /// </summary>
    [SerializeField]
    private ActionBasedController rightController;

    /// <summary>
    /// The left trigger action to check if a left VR trigger was pressed
    /// </summary>
    private InputActionProperty leftTriggerAction;

    /// <summary>
    /// The right trigger action to check if a right VR trigger was pressed
    /// </summary>
    private InputActionProperty rightTriggerAction;

    /// <summary>
    /// The left right input mappings for the keyboard input
    /// </summary>
    private LeftRightInputMappings leftRightMappings;

    /// <summary>
    /// This is used to recognize the left and right keywords for voice commands
    /// </summary>
    private KeywordRecognizer keywordRecognizer;

    /// <summary>
    /// The array of keywords to listen for
    /// </summary>
    [SerializeField]
    private string[] m_Keywords;

    private void Awake()
    {
        leftRightMappings = new LeftRightInputMappings();
    }

    private void Start()
    {
        // Disable the GUI panel at the start
        //guiPanel.SetActive(false);

        // Initialize voice commands
        InitializeVoiceCommands();
    }

    /// <summary>
    /// Called when the gameobject is enabled this will setup all the listners for input.
    /// </summary>
    private void OnEnable()
    {
        // Add click listeners to the buttons
        leftButton.onClick.AddListener(OnLeftButtonClick);
        rightButton.onClick.AddListener(OnRightButtonClick);

        // Get the trigger actions from the controllers
        leftTriggerAction = leftController.activateAction;
        rightTriggerAction = rightController.activateAction;

        // Subscribe the left and right actions to the perfromed events for the VR controllers
        leftTriggerAction.action.performed += OnLeftActionPerformed;
        rightTriggerAction.action.performed += OnRightActionPerformed;

        // Subscribe the left and right actions to the perfromed events for the L and R keyboard buttons
        leftRightMappings.LeftRightInputMap.LeftAction.performed += OnLeftActionPerformed;
        leftRightMappings.LeftRightInputMap.RightAction.performed += OnRightActionPerformed;

        leftRightMappings.Enable();
    }


    private void OnDisable()
    {
        leftTriggerAction.action.performed -= OnLeftActionPerformed;
        rightTriggerAction.action.performed -= OnRightActionPerformed;

        leftRightMappings.LeftRightInputMap.LeftAction.performed -= OnLeftActionPerformed;
        leftRightMappings.LeftRightInputMap.RightAction.performed -= OnRightActionPerformed;

        leftRightMappings.Disable();
    }


    public void ActivateGUI()
    {
        guiActivated = true;
        guiPanel.SetActive(true);

        // Start listening for voice commands when the GUI is activated
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Start();
        }
    }

    /// <summary>
    /// Sets the LeftRightText to say Left. This is used for keyboard and VR input actions
    /// </summary>
    /// <param name="value">The callback context returned by the input event</param>
    public void OnLeftActionPerformed(InputAction.CallbackContext value)
    {
        LeftRightText.text = "Left";
    }

    /// <summary>
    /// Sets the LeftRightText to say Right. This is used for keyboard and VR input actions
    /// </summary>
    /// <param name="value">The callback context returned by the input event</param>
    public void OnRightActionPerformed(InputAction.CallbackContext value)
    {
        LeftRightText.text = "Right";
    }

    /// <summary>
    /// Sets the LeftRightText to say Left.
    /// </summary>
    public void OnLeftButtonClick()
    {
        LeftRightText.text = "Left";
    }

    /// <summary>
    /// Sets the LeftRightText to say Left.
    /// </summary>
    public void OnRightButtonClick()
    {
        LeftRightText.text = "Right";
    }

    /// <summary>
    /// Assigns the keywords to use for voice commands, assigned the function to call when on is detected, and
    /// initializes the keyword recognizer.
    /// </summary>
    private void InitializeVoiceCommands()
    {
        keywordRecognizer = new KeywordRecognizer(m_Keywords);
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        keywordRecognizer.Start();
    }
    /// <summary>
    /// This is called if a keyword is recognized and will display the correct text in the GUI.
    /// </summary>
    /// <param name="args">The event args for the Phrase recognizied event</param>
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if(args.text.ToLower() == "left")
        {
            OnLeftButtonClick();
        }
        else if(args.text.ToLower() == "right")
        {
            OnRightButtonClick();
        }
    }
}
