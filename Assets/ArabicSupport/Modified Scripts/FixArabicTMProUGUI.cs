using UnityEngine;
using TMPro;
using UnityEditor;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class FixArabicTMProUGUI : MonoBehaviour
{
    [SerializeField] // السماح لتعديل النص من المحرر هنا؟ (أي من القيمة text في الأسقل)
    bool editTextHere = false;
    [Multiline(3)]//جعل النص يظهر بأكثر من سطر، يمكن تعديل القيمة لتغيير عدد الأسطر
    [SerializeField]
    string customText;

    [Tooltip("عينها ك true إذا لم ترد التعديل على النص من الملف البرمجي مباشرة(من خاصية ال text في الملف) ")]
    [SerializeField]
    bool textAlreadySet = false;
    [SerializeField]
    bool updateInRealTime = true;
    [Tooltip("تحديث نص العنصر فقط بعد أن تم التوقف عن تغيير الخصائص (أفضل للأداء، ولكن قد يجعل الحركات والتأثيرات أبطئ")]
    [SerializeField]
    bool updateOnlyOnChange = true;
    TextMeshProUGUI textMeshPro;
    [SerializeField]
    bool useTashkeel = false;
    [SerializeField]
    bool useHinduNumbers = false;

    RectTransform rectTransform;
    float previousWorldRectWH;
    string neededText;
    bool previousFrameNeededEdit = false;
    string editedText;
    bool initialized;
    string prevCorrectText;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        textMeshPro = GetComponent<TextMeshProUGUI>();
        neededText = textMeshPro.text;
        initialized = true;
    }

    void Start()
    {

        if (textAlreadySet)
        {
            rectTransform.hasChanged = false;
            prevCorrectText = textMeshPro.FixArabicTMProUGUILines(useTashkeel, useHinduNumbers, neededText);
            textMeshPro.text = prevCorrectText;
        }
        else if (editTextHere)
        {
            UpdateText(customText);
        }

    }

    void Update()
    {
        if (!updateInRealTime)
            return;

        float currentRectArea = rectTransform.rect.height + rectTransform.rect.width;
        if (previousFrameNeededEdit || (!updateOnlyOnChange))
        {
            editedText = textMeshPro.FixArabicTMProUGUILines(useTashkeel, useHinduNumbers, neededText);
            textMeshPro.text = editedText;

            previousFrameNeededEdit = false;
            textMeshPro.havePropertiesChanged = false;
        }
        else if (updateOnlyOnChange &&
             (textMeshPro.havePropertiesChanged || currentRectArea != previousWorldRectWH))
        {
            previousFrameNeededEdit = true;
        }
        previousWorldRectWH = rectTransform.rect.width + rectTransform.rect.height;

    }

    //يجب استخدام هذه الدالة لتغيير النص عند الحاجة
    public void UpdateText(string text)
    {
        if (!initialized)
            Awake();
        neededText = text;
        textMeshPro.text = textMeshPro.FixArabicTMProUGUILines(useTashkeel, useHinduNumbers, text);
    }
    
    public string ReturnFixedString(string text)
    {
        if (!initialized)
            Awake();
        neededText = text;
        text = textMeshPro.FixArabicTMProUGUILines(useTashkeel, useHinduNumbers, text);

        return text; 
    }

    public void ConfigureText()
    {
        TMP_InputField textIF = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        textIF.text = textMeshPro.FixArabicTMProUGUILines(useTashkeel, useHinduNumbers, textMeshPro.text);
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(FixArabicTMProUGUI))]
public class FixArabicTMProUGUIEditor : UnityEditor.Editor
{
    private string previousText;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();
        FixArabicTMProUGUI myScript = (FixArabicTMProUGUI)target;

        string currentText = serializedObject.FindProperty("customText").stringValue;
        if (currentText != previousText)
        {
            myScript.UpdateText(currentText);
        }
        previousText = currentText;
        serializedObject.ApplyModifiedProperties();
    }
}
#endif