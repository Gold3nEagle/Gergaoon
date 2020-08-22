using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SweetSugar.Scripts.LinqConstructor.AnyFieldInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SweetSugar.Scripts.LinqConstructor.Editor
{
    [CustomEditor(typeof(Observer))]
    public class ObserverEditor : UnityEditor.Editor
    {
        Observer myTarget;
        private int selectedComponent;
        private List<AnyField> objectPath;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            myTarget = (Observer)target;
            // var obj = (GameObject)myTarget.obj;
            ShowButtons(objectPath);
            if (myTarget.obj != null)
                ObserveGameObject((GameObject)myTarget.obj);
            else if (myTarget.obj2 != null)
                ObserveObject(myTarget.obj2);

            SearchObjects(objectPath);
        }

        #region System_Object

        private void ObserveObject(object obj)
        {
            GUILayout.Label("observing " + obj);

            objectPath = myTarget.objectPath;

            for (int i = 0; i < objectPath.Count; i++)
            {
                SelectFieldsObject(objectPath[i]);
            }
            // ShowValue(objectPath);
        }



        private void SelectFieldsObject(AnyField getObj)
        {
            if (getObj == null) return;

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            // MemberInfo[] members = getObj.obj.GetType().GetFields(bindingFlags).Cast<MemberInfo>()
            //     .Concat(getObj.obj.GetType().GetProperties(bindingFlags)).ToArray();
            var fields = getObj.obj.GetType().GetFields(bindingFlags);
            var props = getObj.obj.GetType().GetProperties(bindingFlags);

            if (fields.Length > 0)
            {
                ShowFields(fields, getObj);
            }
            // if (props.Length > 0)
            // {
            //     ShowProperity(props, getObj);
            // }
            else
                GUILayout.Label(getObj.obj + "= " + getObj.value);
            if (getObj.obj.GetType() == typeof(GameObject))
            {
                var components = (getObj.obj as GameObject).GetComponents<MonoBehaviour>();
                foreach (var item in components)
                {
                    AnyField listObject1 = getObj.objectPath.Find(x => x.obj.Equals(item));
                    if (listObject1 == null)
                        getObj.objectPath.Add(new AnyField(item, 0, item.ToString()));
                    SelectFieldsObject(listObject1);
                }
            }
            if (getObj.obj.GetType() is IEnumerable)
            {
                Debug.Log(getObj.obj.GetType());

                IEnumerable enumerable = (IEnumerable)getObj.obj;
                foreach (var item2 in enumerable)
                {
                    AnyField listObject1 = getObj.objectPath.Find(x => x.obj.Equals(item2));
                    if (listObject1 == null)
                        getObj.objectPath.Add(new AnyField(item2, 0, item2.ToString()));
                    SelectFieldsObject(listObject1);
                }
            }
        }

        private void ShowProperity(PropertyInfo[] components, AnyField listObject)
        {
            listObject.expand = EditorGUILayout.Foldout(listObject.expand, listObject.obj + "= " + listObject.value + ":");
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                if (listObject.expand)
                {
                    GUILayout.BeginVertical();
                    {
                        foreach (var item in components)
                        {
                            AnyField listObject1 = listObject.objectPath.Find(x => x.obj.Equals(item));
                            if (listObject1 == null)
                                listObject.objectPath.Add(new AnyField(item, 0, item.GetValue(listObject.obj)?.ToString()));
                            SelectFieldsObject(listObject1);
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ShowFields(FieldInfo[] components, AnyField listObject)
        {
            listObject.expand = EditorGUILayout.Foldout(listObject.expand, listObject.obj + "= " + listObject.value + ":");
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                if (listObject.expand)
                {
                    GUILayout.BeginVertical();
                    {
                        foreach (var item in components)
                        {
                            AnyField listObject1 = listObject.objectPath.Find(x => x.obj.Equals(item));
                            if (listObject1 == null)
                                listObject.objectPath.Add(new AnyField(item, 0, item.GetValue(listObject.obj)?.ToString()));
                            SelectFieldsObject(listObject1);
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region GameObject

        private void ObserveGameObject(GameObject obj)
        {
            objectPath = myTarget.objectPath;
            if (objectPath.Count == 0) return;
            SelectComponent(obj, objectPath[0]);
            for (int i = 1; i < objectPath.Count; i++)
            {
                if (objectPath[i - 1].obj is GameObject)
                    SelectComponent(objectPath[i - 1].obj as GameObject, objectPath[i]);
                else
                    SelectField(objectPath[i - 1], objectPath[i]);
            }
            //ShowValue(objectPath);
        }
        private void SelectField(AnyField getObj, AnyField setObj)
        {
            //if (setObj.Equals(objectPath.LastOrDefault()))
            //Debug.Log(getObj.obj);
            FieldInfo[] fields = getObj.obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            // PropertyInfo[] properties = GetProperties(getObj.obj) as PropertyInfo[];
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            MemberInfo[] members = getObj.obj.GetType().GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(getObj.obj.GetType().GetProperties(bindingFlags)).ToArray();
            if (members.Length > 0)
            {
                ShowEnum(members, setObj, getObj);
            }
        }

        object GetValue(MemberInfo member, object obj)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.GetValue(obj);
            var property = member as PropertyInfo;
            if (property != null)
                return property.GetValue(obj);

            throw new NotSupportedException($"The type '{member.GetType()}' is not supported.");
        }



        private void ShowEnum(MemberInfo[] components, AnyField listObject, AnyField getObj)
        {
            GUILayout.BeginHorizontal();
            {
                listObject.index = EditorGUILayout.Popup(listObject.index, components.Select(i => i.Name).ToArray());
                //listObject.objectPath.Add(new ListObject(components[listObject.index], listObject.index, GetValue(components[listObject.index], getObj.obj)));
                listObject.obj = components[listObject.index];
                listObject.value = GetValue(components[listObject.index], getObj.obj);
                listObject.nameObj = components[listObject.index].Name;
                ShowValue(listObject);
            }
            GUILayout.EndHorizontal();

        }
        private void SelectComponent(GameObject obj, AnyField listObject)
        {
            var components = obj.GetComponents<MonoBehaviour>();
            ShowEnumComponent(components, listObject);
        }

        private void ShowEnumComponent(object[] components, AnyField listObject)
        {
            listObject.index = EditorGUILayout.Popup(listObject.index, components.Select(i => i.GetType().ToString()).ToArray());
            listObject.obj = components[listObject.index];
        }
        #endregion


        private static void ShowButtons(List<AnyField> objectPath)
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("+"))
                {
                    objectPath.Add(new AnyField());
                }
                if (GUILayout.Button("-"))
                {
                    objectPath.RemoveAt(objectPath.Count - 1);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void ShowValue(AnyField obj)
        {
            if (obj.value is IEnumerable)
            {
                string line = "";
                var list = obj.value as IEnumerable;
                if (list is string) line = list.ToString();
                else
                {
                    foreach (var item in list)
                    {
                        line += item + Environment.NewLine;
                    }
                }
                GUILayout.Label(line);
            }
            else
                GUILayout.Label(obj?.value?.ToString() ?? "null");

        }


        #region Search


        string searchValue = "";
        List<object> foundObjects = new List<object>();
        private void SearchObjects(List<AnyField> objectPath)
        {
            searchValue = GUILayout.TextArea(searchValue);
            if (GUILayout.Button("Find"))
            {
                StartFind(objectPath);
            }
            foreach (var item in foundObjects)
            {
                EditorGUILayout.ObjectField((Object)item, typeof(object), true);

            }

        }

        private void StartFind(List<AnyField> objectPath)
        {
            foundObjects.Clear();
            var go = FindObjectsOfType(objectPath[0].obj.GetType());
            foreach (var item in go)
            {
                if (GetValue((MemberInfo)objectPath.LastOrDefault().obj, item).ToString() == searchValue)
                {
                    foundObjects.Add(item);
                }
            }
        }

        #endregion

        bool hooked;
        #region Hook
        void Hook()
        {
            if (GUILayout.Button(!hooked ? "Hook" : "Unhook"))
            {
                hooked = !hooked;
                //if (hooked) Startc
            }
        }

        IEnumerator CheckHook()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                StartFind(objectPath);
            }
        }

        #endregion

    }
}