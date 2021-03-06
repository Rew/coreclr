// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Reflection
{
    internal static class MdConstant
    {
        public static unsafe object? GetValue(MetadataImport scope, int token, RuntimeTypeHandle fieldTypeHandle, bool raw)
        {
            CorElementType corElementType = 0;
            long buffer = 0;
            int length;
            string? stringVal;

            stringVal = scope.GetDefaultValue(token, out buffer, out length, out corElementType);

            RuntimeType fieldType = fieldTypeHandle.GetRuntimeType();

            if (fieldType.IsEnum && raw == false)
            {
                // NOTE: Unlike in `TypeBuilder.SetConstantValue`, if `fieldType` describes
                // a nullable enum type `Nullable<TEnum>`, we do not unpack it to `TEnum` to
                // successfully enter this `if` clause. Default values of `TEnum?`-typed
                // parameters have been reported as values of the underlying type, changing
                // this now might be a breaking change.

                long defaultValue = 0;

                switch (corElementType)
                {
                    #region Switch

                    case CorElementType.ELEMENT_TYPE_VOID:
                        return DBNull.Value;

                    case CorElementType.ELEMENT_TYPE_CHAR:
                        defaultValue = *(char*)&buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_I1:
                        defaultValue = *(sbyte*)&buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_U1:
                        defaultValue = *(byte*)&buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_I2:
                        defaultValue = *(short*)&buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_U2:
                        defaultValue = *(ushort*)&buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_I4:
                        defaultValue = *(int*)&buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_U4:
                        defaultValue = *(uint*)&buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_I8:
                        defaultValue = buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_U8:
                        defaultValue = buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_CLASS:
                        return null;

                    default:
                        throw new FormatException(SR.Arg_BadLiteralFormat);
                        #endregion
                }

                return RuntimeType.CreateEnum(fieldType, defaultValue);
            }
            else if (fieldType == typeof(DateTime))
            {
                long defaultValue = 0;

                switch (corElementType)
                {
                    #region Switch

                    case CorElementType.ELEMENT_TYPE_VOID:
                        return DBNull.Value;

                    case CorElementType.ELEMENT_TYPE_I8:
                        defaultValue = buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_U8:
                        defaultValue = buffer;
                        break;

                    case CorElementType.ELEMENT_TYPE_CLASS:
                        return null;

                    default:
                        throw new FormatException(SR.Arg_BadLiteralFormat);
                        #endregion
                }

                return new DateTime(defaultValue);
            }
            else
            {
                return corElementType switch
                {
                    CorElementType.ELEMENT_TYPE_VOID => DBNull.Value,
                    CorElementType.ELEMENT_TYPE_CHAR => *(char*)&buffer,
                    CorElementType.ELEMENT_TYPE_I1 => *(sbyte*)&buffer,
                    CorElementType.ELEMENT_TYPE_U1 => *(byte*)&buffer,
                    CorElementType.ELEMENT_TYPE_I2 => *(short*)&buffer,
                    CorElementType.ELEMENT_TYPE_U2 => *(ushort*)&buffer,
                    CorElementType.ELEMENT_TYPE_I4 => *(int*)&buffer,
                    CorElementType.ELEMENT_TYPE_U4 => *(uint*)&buffer,
                    CorElementType.ELEMENT_TYPE_I8 => buffer,
                    CorElementType.ELEMENT_TYPE_U8 => (ulong)buffer,
                    CorElementType.ELEMENT_TYPE_BOOLEAN => (*(int*)&buffer != 0),
                    CorElementType.ELEMENT_TYPE_R4 => *(float*)&buffer,
                    CorElementType.ELEMENT_TYPE_R8 => *(double*)&buffer,
                    CorElementType.ELEMENT_TYPE_STRING => stringVal ?? string.Empty,
                    CorElementType.ELEMENT_TYPE_CLASS => null,
                    _ => throw new FormatException(SR.Arg_BadLiteralFormat),
                };
            }
        }
    }
}
