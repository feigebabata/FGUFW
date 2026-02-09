using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace FGUFW.Console
{
    [ExecuteInEditMode]
    public class ConsoleWindow : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _MsgView,_inputField;


        [SerializeField]
        private Color _defaultTextColor,_commandTextColor,_variableTextColor,_errorTextColor;

        private string _defaultTextColorText,_commandTextColorText,_variableTextColorText,_errorTextColorText;


        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {

            _defaultTextColorText = ColorUtility.ToHtmlStringRGB(_defaultTextColor);
            _commandTextColorText = ColorUtility.ToHtmlStringRGB(_commandTextColor);
            _variableTextColorText = ColorUtility.ToHtmlStringRGB(_variableTextColor);
            _errorTextColorText = ColorUtility.ToHtmlStringRGB(_errorTextColor);

            onAddListener();
        }

        IEnumerator Start()
        {
            yield return new WaitWhile(()=>ConsoleUtility.Initialized);
            onInputSubmit("help");
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            onRemoveListener();
        }

        private void onAddListener()
        {
            _inputField.onSubmit.AddListener(onInputSubmit);

            ConsoleUtility.OnAddDefCommandMsg += onAddDefCommandMsg;
            ConsoleUtility.OnAddSetCommandMsg += onAddSetCommandMsg;
            ConsoleUtility.OnAddInvokeResultMsg += onAddInvokeResultMsg;
            ConsoleUtility.OnAddInvokeFailMsg += onAddInvokeFailMsg;
        }

        private void onRemoveListener()
        {
            _inputField.onSubmit.RemoveAllListeners();

            ConsoleUtility.OnAddDefCommandMsg -= onAddDefCommandMsg;
            ConsoleUtility.OnAddSetCommandMsg -= onAddSetCommandMsg;
            ConsoleUtility.OnAddInvokeResultMsg -= onAddInvokeResultMsg;
            ConsoleUtility.OnAddInvokeFailMsg -= onAddInvokeFailMsg;
            
        }

        private string onAddDefCommandMsg(string arg)
        {
            return $"<color=#{_commandTextColorText}>{arg}";
        }

        private string onAddSetCommandMsg(string arg)
        {
            var args = arg.Split('=');
            return $"<color=#{_variableTextColorText}>{args[0]}=<color=#{_commandTextColorText}>{args[1]}";
        }

        private string onAddInvokeResultMsg(string arg)
        {
            return $"<color=#{_defaultTextColorText}>{arg}";
        }

        private string onAddInvokeFailMsg(string arg)
        {
            return $"<color=#{_errorTextColorText}>{arg}";
        }

        private void onInputSubmit(string arg0)
        {
            // Debug.Log($"发送:{arg0}");

            ConsoleUtility.ParseCommand(arg0);
            _MsgView.text = ConsoleUtility.ConloseAllMsg;

            _inputField.text = default;
            _inputField.ActivateInputField();
        }






    }


}
