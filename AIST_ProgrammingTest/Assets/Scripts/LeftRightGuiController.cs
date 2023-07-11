using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

namespace Test2
{
    /// <summary>
    /// This class handles displaying the left or right text depending on user input
    /// </summary>
    public class LeftRightGuiController : MonoBehaviour
    {
        /// <summary>
        /// The Gui panel to display the left and right text and left and right buttons
        /// </summary>
        [SerializeField]
        private GameObject guiPanel;

        /// <summary>
        /// The text to display left and right depending on the input
        /// </summary>
        [SerializeField]
        private TMP_Text LeftRightText;

        /// <summary>
        /// The left button to press to display left text
        /// </summary
        [SerializeField]
        private Button leftButton;

        /// <summary>
        /// The right button to press to display right text
        /// </summary>
        [SerializeField]
        private Button rightButton;

        /// <summary>
        /// The toggle that controls the Left Right Gui visibility
        /// </summary>
        [SerializeField]
        private Toggle GUIToggle;

        /// <summary>
        /// The text to display on the GUI toggle
        /// </summary>
        [SerializeField]
        private TMP_Text toggleText;

        /// <summary>
        /// The canvas group to control the visibility of the left right gui
        /// </summary>
        [SerializeField]
        private CanvasGroup GUICanvaseGroup;

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

        /// <summary>
        /// Initializes void commands and sets up the mouse functionality for the buttons and toggle in the GUI
        /// </summary>
        private void Start()
        {
            // Initialize voice commands
            InitializeVoiceCommands();

            // Add click listeners to the buttons
            leftButton.onClick.AddListener(OnLeftButtonClick);
            rightButton.onClick.AddListener(OnRightButtonClick);

            GUIToggle.onValueChanged.AddListener(ToggleGUI);
        }

        /// <summary>
        /// Called when the gameobject is enabled this will setup all the listners for input.
        /// </summary>
        private void OnEnable()
        {
            // Get the trigger actions from the controllers
            leftTriggerAction = leftController.activateAction;
            rightTriggerAction = rightController.activateAction;

            // Subscribe the left and right actions to the perfromed events for the VR controllers
            leftTriggerAction.action.performed += OnLeftActionPerformed;
            rightTriggerAction.action.performed += OnRightActionPerformed;

            // Subscribe the left and right actions to the perfromed events for the L and R keyboard buttons
            leftRightMappings.LeftRightInputMap.LeftAction.performed += OnLeftActionPerformed;
            leftRightMappings.LeftRightInputMap.RightAction.performed += OnRightActionPerformed;

            // Subscribe the open and close actions to the performed events for the O and C keyboard buttons
            leftRightMappings.OpenCloseInputMap.Open.performed += OnOpenActionPerformed;
            leftRightMappings.OpenCloseInputMap.Close.performed += OnCloseActionPerformed;

            leftRightMappings.Enable();
        }

        /// <summary>
        /// When this Gameobject is disabled unsubscribe from the performed events and siable the left right mappings
        /// </summary>
        private void OnDisable()
        {
            leftTriggerAction.action.performed -= OnLeftActionPerformed;
            rightTriggerAction.action.performed -= OnRightActionPerformed;

            leftRightMappings.LeftRightInputMap.LeftAction.performed -= OnLeftActionPerformed;
            leftRightMappings.LeftRightInputMap.RightAction.performed -= OnRightActionPerformed;

            leftRightMappings.OpenCloseInputMap.Open.performed -= OnOpenActionPerformed;
            leftRightMappings.OpenCloseInputMap.Close.performed -= OnCloseActionPerformed;

            leftRightMappings.Disable();
        }

        /// <summary>
        /// Sets the LeftRightText to say Left. This is used for keyboard and VR input actions
        /// </summary>
        /// <param name="value">The callback context returned by the input event</param>
        public void OnLeftActionPerformed(InputAction.CallbackContext value)
        {
            OnLeftButtonClick();
        }

        /// <summary>
        /// Sets the LeftRightText to say Right. This is used for keyboard and VR input actions
        /// </summary>
        /// <param name="value">The callback context returned by the input event</param>
        public void OnRightActionPerformed(InputAction.CallbackContext value)
        {
            OnRightButtonClick();
        }

        /// <summary>
        /// Toggles on the GUI when the open action is performed.
        /// </summary>
        /// <param name="value">The callback context returned by the input event</param>
        public void OnOpenActionPerformed(InputAction.CallbackContext value)
        {
            GUIToggle.isOn = true;
        }

        /// <summary>
        /// Toggles off the GUI when the open action is performed.
        /// </summary>
        /// <param name="value">The callback context returned by the input event</param>
        public void OnCloseActionPerformed(InputAction.CallbackContext value)
        {
            GUIToggle.isOn = false;
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
        /// This is called if a keyword is recognized and will display the correct text in the GUI or toggle the 
        /// GUI toggle to hide or show the left right GUI.
        /// </summary>
        /// <param name="args">The event args for the Phrase recognizied event</param>
        private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            if (args.text.ToLower() == "left")
            {
                OnLeftButtonClick();
            }
            else if (args.text.ToLower() == "right")
            {
                OnRightButtonClick();
            }
            else if (args.text.ToLower() == "open")
            {
                GUIToggle.isOn = true;
            }
            else if (args.text.ToLower() == "close")
            {
                GUIToggle.isOn = false;
            }
        }

        /// <summary>
        /// Handles changing the state of the left right GUI so that the left right buttons and the text are only
        /// displayed when the toggle state is true.
        /// </summary>
        /// <param name="state">The state of the GUI Toggle</param>
        private void ToggleGUI(bool state)
        {
            if (state)
            {
                toggleText.text = "Close";

                GUICanvaseGroup.alpha = 1;

                GUICanvaseGroup.interactable = true;
            }
            else
            {
                toggleText.text = "Open";

                GUICanvaseGroup.alpha = 0;

                GUICanvaseGroup.interactable = false;
            }
        }
    }
}
