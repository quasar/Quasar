using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// A resource Id.
    /// There're two types of resource Ids, reserved integer numbers (eg. RT_ICON) and custom string names (eg. "CUSTOM").
    /// </summary>
    public class ResourceId
    {
        private IntPtr _name = IntPtr.Zero;

        /// <summary>
        /// A resource identifier.
        /// </summary>
        /// <param name="value">A integer or string resource id.</param>
        public ResourceId(IntPtr value)
        {
            Id = value;
        }

        /// <summary>
        /// A resource identifier.
        /// </summary>
        /// <param name="value">A integer resource id.</param>
        public ResourceId(uint value)
        {
            Id = new IntPtr(value);
        }

        /// <summary>
        /// A well-known resource-type identifier.
        /// </summary>
        /// <param name="value">A well known resource type.</param>
        public ResourceId(Kernel32.ResourceTypes value)
        {
            Id = (IntPtr) value;
        }

        /// <summary>
        /// A custom resource identifier.
        /// </summary>
        /// <param name="value"></param>
        public ResourceId(string value)
        {
            Name = value;
        }

        /// <summary>
        /// Resource Id.
        /// </summary>
        /// <remarks>
        /// If the resource Id is a string, it will be copied.
        /// </remarks>
        public IntPtr Id
        {
            get
            {
                return _name;
            }
            set
            {
                _name = IsIntResource(value)
                    ? value
                    : Marshal.StringToHGlobalUni(Marshal.PtrToStringUni(value));
            }
        }

        /// <summary>
        /// String representation of a resource type name.
        /// </summary>
        public string TypeName
        {
            get
            {
                return IsIntResource() ? ResourceType.ToString() : Name;
            }
        }

        /// <summary>
        /// An enumerated resource type for built-in resource types only.
        /// </summary>
        public Kernel32.ResourceTypes ResourceType
        {
            get
            {
                if (IsIntResource())
                    return (Kernel32.ResourceTypes) _name;

                throw new InvalidCastException(string.Format(
                    "Resource {0} is not of built-in type.", Name));
            }
            set
            {
                _name = (IntPtr) value;
            }
        }

        /// <summary>
        /// Returns true if the resource is an integer resource.
        /// </summary>
        public bool IsIntResource()
        {
            return IsIntResource(_name);
        }

        /// <summary>
        /// Returns true if the resource is an integer resource.
        /// </summary>
        /// <param name="value">Resource pointer.</param>
        internal static bool IsIntResource(IntPtr value)
        {
            return value.ToInt64() <= UInt16.MaxValue;
        }

        /// <summary>
        /// Resource Id in a string format.
        /// </summary>
        public string Name
        {
            get
            {
                return IsIntResource()
                    ? _name.ToString()
                    : Marshal.PtrToStringUni(_name);
            }
            set
            {
                _name = Marshal.StringToHGlobalUni(value);
            }
        }

        /// <summary>
        /// String representation of the resource Id.
        /// </summary>
        /// <returns>Resource name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Resource Id hash code. 
        /// Resource Ids of the same type have the same hash code.
        /// </summary>
        /// <returns>Resource Id.</returns>
        public override int GetHashCode()
        {
            return IsIntResource() 
                ? Id.ToInt32() 
                : Name.GetHashCode();
        }

        /// <summary>
        /// Compares two resource Ids by value.
        /// </summary>
        /// <param name="obj">Resource Id.</param>
        /// <returns>True if both resource Ids represent the same resource.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ResourceId && obj == this)
                return true;

            if (obj is ResourceId && (obj as ResourceId).GetHashCode() == GetHashCode())
                return true;

            return false;
        }
    }
}
