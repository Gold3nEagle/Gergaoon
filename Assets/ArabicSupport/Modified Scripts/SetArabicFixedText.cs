using UnityEngine;
using UnityEngine.UI;
using System;
[RequireComponent(typeof(Text))]
public class SetArabicFixedText : MonoBehaviour
{


    [SerializeField]
    bool useTashkeel;
    [SerializeField]
    bool useHinduNumbers;
    [SerializeField]
    bool updateOnRealtime = true;

    private bool hasChangedLastFrame;
    private float previousWorldRectWH;
    private string neededText;
    private RectTransform rectTransform;
    Text UIText; //نعرف متغير يحمل كائن/عنصر النص 

    void Start()
    {
        UIText = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();
        if (UIText.text != string.Empty) // إذا لم يكن النص فارغًا
        {
            //نعرف متغير نصي، ونعين قيمته عن طريق استدعاء
            // دالة من الإضافة
            //هذه الدالة المستدعاة تأخذ النص العربي،
            // تقوم بإعادة ضبطه ليصبح جاهزًا للعرض،
            // ثم ترجعه كقيمة نصية
            neededText = UIText.text;
            UIText.text = UIText.FixArabicUITextLines(useTashkeel, useHinduNumbers, neededText);
        }

        //نجمع طول وعرض عنصر النص ونحفظهم في المتغير..
        //لنستطيع بعد ذلك ملاحظة التغيير في الأبعاد. 
        previousWorldRectWH = rectTransform.rect.width + rectTransform.rect.height;
    }

    void Update()
    {
        if (!updateOnRealtime)
            return;

        //نحسب مجموع طول وعرض عنصر النص في كل إطار Frame
        float currentRectWH = rectTransform.rect.width + rectTransform.rect.height;
        //إذا حصل تغيير في الأبعاد في الإطار السابق، ولكن لم يحصل أي تغيير بين الإطار السابق والإطار الحالي:
        if (hasChangedLastFrame && previousWorldRectWH == currentRectWH)
        {
            //نقوم بإعادة ترتيب النص 
            UIText.text = UIText.FixArabicUITextLines(useTashkeel, useHinduNumbers, neededText);
        }
        hasChangedLastFrame = currentRectWH != previousWorldRectWH;
        previousWorldRectWH = rectTransform.rect.width + rectTransform.rect.height;
    }

    //يجب استخدام هذه الدالة لتغيير النص عند الحاجة
    public void UpdateText(string text)
    {
        neededText = text;
        UIText.text = UIText.FixArabicUITextLines(useTashkeel, useHinduNumbers, text);
    }

}