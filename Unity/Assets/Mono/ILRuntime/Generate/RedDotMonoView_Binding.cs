using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class RedDotMonoView_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::RedDotMonoView);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("Show", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Show_0);
            args = new Type[]{};
            method = type.GetMethod("Recovery", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Recovery_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("RefreshRedDotCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RefreshRedDotCount_2);

            field = type.GetField("isRedDotActive", flag);
            app.RegisterCLRFieldGetter(field, get_isRedDotActive_0);
            app.RegisterCLRFieldSetter(field, set_isRedDotActive_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_isRedDotActive_0, AssignFromStack_isRedDotActive_0);
            field = type.GetField("RedDotScale", flag);
            app.RegisterCLRFieldGetter(field, get_RedDotScale_1);
            app.RegisterCLRFieldSetter(field, set_RedDotScale_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_RedDotScale_1, AssignFromStack_RedDotScale_1);
            field = type.GetField("PositionOffset", flag);
            app.RegisterCLRFieldGetter(field, get_PositionOffset_2);
            app.RegisterCLRFieldSetter(field, set_PositionOffset_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_PositionOffset_2, AssignFromStack_PositionOffset_2);


        }


        static StackObject* Show_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @redDotGameObject = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::RedDotMonoView instance_of_this_method = (global::RedDotMonoView)typeof(global::RedDotMonoView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Show(@redDotGameObject);

            return __ret;
        }

        static StackObject* Recovery_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::RedDotMonoView instance_of_this_method = (global::RedDotMonoView)typeof(global::RedDotMonoView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Recovery();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* RefreshRedDotCount_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @count = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::RedDotMonoView instance_of_this_method = (global::RedDotMonoView)typeof(global::RedDotMonoView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RefreshRedDotCount(@count);

            return __ret;
        }


        static object get_isRedDotActive_0(ref object o)
        {
            return ((global::RedDotMonoView)o).isRedDotActive;
        }

        static StackObject* CopyToStack_isRedDotActive_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::RedDotMonoView)o).isRedDotActive;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isRedDotActive_0(ref object o, object v)
        {
            ((global::RedDotMonoView)o).isRedDotActive = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isRedDotActive_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isRedDotActive = ptr_of_this_method->Value == 1;
            ((global::RedDotMonoView)o).isRedDotActive = @isRedDotActive;
            return ptr_of_this_method;
        }

        static object get_RedDotScale_1(ref object o)
        {
            return ((global::RedDotMonoView)o).RedDotScale;
        }

        static StackObject* CopyToStack_RedDotScale_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::RedDotMonoView)o).RedDotScale;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_RedDotScale_1(ref object o, object v)
        {
            ((global::RedDotMonoView)o).RedDotScale = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_RedDotScale_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @RedDotScale = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @RedDotScale, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @RedDotScale = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }
            ((global::RedDotMonoView)o).RedDotScale = @RedDotScale;
            return ptr_of_this_method;
        }

        static object get_PositionOffset_2(ref object o)
        {
            return ((global::RedDotMonoView)o).PositionOffset;
        }

        static StackObject* CopyToStack_PositionOffset_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::RedDotMonoView)o).PositionOffset;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_PositionOffset_2(ref object o, object v)
        {
            ((global::RedDotMonoView)o).PositionOffset = (UnityEngine.Vector2)v;
        }

        static StackObject* AssignFromStack_PositionOffset_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2 @PositionOffset = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @PositionOffset, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @PositionOffset = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)16);
            }
            ((global::RedDotMonoView)o).PositionOffset = @PositionOffset;
            return ptr_of_this_method;
        }



    }
}
