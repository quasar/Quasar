using System;

namespace ProtoBuf
{
    /// <summary>
    /// Declares a member to be used in protocol-buffer serialization, using
    /// the given Tag and MemberName. This allows ProtoMemberAttribute usage
    /// even for partial classes where the individual members are not
    /// under direct control.
    /// A DataFormat may be used to optimise the serialization
    /// format (for instance, using zigzag encoding for negative numbers, or 
    /// fixed-length encoding for large values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,
        AllowMultiple = true, Inherited = false)]
    public class ProtoPartialMemberAttribute : ProtoMemberAttribute
    {
        /// <summary>
        /// Creates a new ProtoMemberAttribute instance.
        /// </summary>
        /// <param name="tag">Specifies the unique tag used to identify this member within the type.</param>
        /// <param name="memberName">Specifies the member to be serialized.</param>
        public ProtoPartialMemberAttribute(int tag, string memberName)
            : base(tag)
        {
            if (Helpers.IsNullOrEmpty(memberName)) throw new ArgumentNullException("memberName");
            this.memberName = memberName;
        }

        /// <summary>
        /// The name of the member to be serialized.
        /// </summary>
        public string MemberName
        {
            get { return memberName; }
        }

        private readonly string memberName;
    }
}