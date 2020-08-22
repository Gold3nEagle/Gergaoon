// using UnityEngine;
// using System.Linq;
// using System.Reflection;
// using System.Diagnostics;
// using System.Collections.Generic;
// using System;

// [ExecuteInEditMode]
// public class Breakpoints : MonoBehaviour
// {
//     void OnEnable()
//     {

//         var trace = new System.Diagnostics.StackTrace(true);
//         Debugger.Array(trace.GetFrames().Select(i => i.GetMethod()).ToArray());
//         var frame = trace.GetFrame(0);
//         var methodName = frame.GetMethod().Name;
//         var properties = this.GetType().GetProperties();
//         var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance); // public fields
//                                                                                             // for example:
//         TestGetLocalVars();
//         foreach (var prop in properties)
//         {
//             try
//             {
//                 var value = prop.GetValue();
//                 UnityEngine.Debug.Log(prop.GetType() + " " + prop.Name + " " + prop.GetValue(this));

//             }
//             catch (System.Exception)
//             {

//             }
//         }
//         foreach (var field in fields)
//         {
//             var value = field.GetValue(this);
//             UnityEngine.Debug.Log(field.GetType() + " " + field.Name + " " + field.GetValue(this));


//         }
//         //     foreach (string key in Session)
//         //     {
//         //         var value = Session[key];
//         //     }
//     }

//     public static void TestGetLocalVars()
//     {
//         Process current = Process.GetCurrentProcess();

//         // Get the path of the current module.
//         string path = current.MainModule.FileName;

//         // Get all local var info for the CSharpRecipes.Reflection.GetLocalVars method.
//         System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo> vars =
//             (System.Collections.ObjectModel.ReadOnlyCollection<LocalVariableInfo>)
//             GetLocalVars(path, "CSharpRecipes.Reflection", "GetLocalVars");
//     }

//     public static IList<LocalVariableInfo> GetLocalVars(string asmPath,
//                                         string typeName, string methodName)
//     {
//         Assembly asm = Assembly.LoadFrom(asmPath);
//         Type asmType = asm.GetType(typeName);
//         MethodInfo mi = asmType.GetMethod(methodName);
//         MethodBody mb = mi.GetMethodBody();

//         IList<LocalVariableInfo> vars = mb.LocalVariables;

//         // Display information about each local variable.
//         foreach (LocalVariableInfo lvi in vars)
//         {
//             UnityEngine.Debug.Log("IsPinned: " + lvi.IsPinned);
//             UnityEngine.Debug.Log("LocalIndex: " + lvi.LocalIndex);
//             UnityEngine.Debug.Log("LocalType.Module: " + lvi.LocalType.Module);
//             UnityEngine.Debug.Log("LocalType.FullName: " + lvi.LocalType.FullName);
//             UnityEngine.Debug.Log("ToString(): " + lvi.ToString());
//         }

//         return (vars);
//     }

// }