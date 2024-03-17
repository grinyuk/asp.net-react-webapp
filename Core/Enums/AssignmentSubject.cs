using System.Runtime.Serialization;

namespace Core.Enums;

public enum AssignmentSubject
{
    [EnumMember(Value = "math")]
    Math,
    [EnumMember(Value = "physics")]
    Physics
}