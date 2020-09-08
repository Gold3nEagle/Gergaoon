using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

//تمت كتابة الملف من قبل الموقع مراحل - لمطوري الألعاب
//http://www.devjourney.epizy.com/النصوص-باللغة-العربية-محرك-unity
//يمكن استخدامه في أي مشروع من دون ذكر المرجع
//ولكن لا يسمح بيعه على حدة
//فهو متوفر بشكل مجاني للجميع

public static class CustomTextFunctions
{
    public static string FixArabicUITextLines(this Text textUIItem, bool useTashkeel, bool hinduNumbers, string text)
    {
        textUIItem.text = text;
        TextGenerator textGenerator = GetTextGenerator(textUIItem, text);

        string reversedText = "";
        string tempLine;
        //حلقة تكرارية بعدد الأسطر الموجودة
        for (int i = 0; i < textGenerator.lineCount; i++)
        {
            //للأسف، لا يتم توفير نص كل سطر بشكل مباشر
            //يجب علينا استخراجه بأنفسنا ^_^
            //سنحصل على موقع بداية ونهاية كل سطر، ثم نقصه من النص الأصلي
            //موقع البداية للسطر هو بكل بساطة:
            int startIndex = textGenerator.lines[i].startCharIdx;
            //أما موقع النهاية: 
            //إذا لم نصل لآخر سطر بعد
            int endIndex = i < textGenerator.lines.Count - 1 ?
                //نحصل على  موقع نهاية السطر من موقع بداية السطر اللاحق:
                textGenerator.lines[i + 1].startCharIdx :
                //ولكن في السطر الأخير، سنحصل عليه من طول النص كاملاً:
                textUIItem.text.Length;
            //نحصل على نص السطر الحالي، وهو معرف بموقعي البداية والنهاية
            //دالة Substring تقتص جزء من نص 
            //استنادًا إلى موقع البداية والطول
            tempLine = textUIItem.text.Substring(startIndex, endIndex - startIndex);
            //نقوم بإصلاح هذا السطر ثم إضافته إلى النص المعكوس النهائي
            reversedText += ArabicSupport.ArabicFixer.Fix(tempLine, useTashkeel, hinduNumbers).Trim('\n');
            reversedText += Environment.NewLine;//نفصل بين كل سطر بمسافة enter
        }
        return reversedText;
    }

    public static TextGenerator GetTextGenerator(Text textUI, string text)
    {
        textUI.text = text; //تعيين النص للعنصر النصي  
                            //تعريف كائن من النوع TextGenerator، الذي سيحتوي على المعلومات
        TextGenerator textGenerator = new TextGenerator();
        //نعرف كائن آخر سيحتوي على أبعاد العنصر text في الواجهة
        //السبب أن عدد الأسطر الجديدة المطلوبة يعتمد على المساحة المتوفرة
        TextGenerationSettings generationSettings = textUI.GetGenerationSettings(textUI.rectTransform.rect.size);
        //نقدم النص والإعدادات لل textGenerator ،
        // ليقوم بحساب عدد الأسطر وما إلى ذلك،
        // عن طريق دالة Populate
        textGenerator.Populate(text, generationSettings);
        //إرجاع الكائن الذي يحتوي على المعلومات المطلوبة
        return textGenerator;
    }

    public static string FixArabicTMProUGUILines(this TextMeshProUGUI textMesh, bool useTashkeel, bool hinduNumbers, string text)
    {
        textMesh.text = text;
        Canvas.ForceUpdateCanvases();
        textMesh.text = text;
        TMP_TextInfo newTextInfo = textMesh.GetTextInfo(text);

        string reversedText = "";
        string tempLine;
        for (int i = 0; i < newTextInfo.lineCount; i++)
        {
            int startIndex = newTextInfo.lineInfo[i].firstCharacterIndex;

            tempLine = text.Substring(startIndex, newTextInfo.lineInfo[i].characterCount);
            reversedText += ArabicSupport.ArabicFixer.Fix(tempLine, useTashkeel, hinduNumbers).Trim('\n');
            reversedText += Environment.NewLine;
        }
        return reversedText;
    }

}