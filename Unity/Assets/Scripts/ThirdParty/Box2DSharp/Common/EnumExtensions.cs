using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;

namespace Box2DSharp.Common
{
    public static class EnumExtensions
    {
        public static bool IsSet(this BodyFlags self, BodyFlags flag) => (self & flag) == flag;

        public static bool IsSet(this DrawFlag self, DrawFlag flag) => (self & flag) == flag;

        public static bool IsSet(this Contact.ContactFlag self, Contact.ContactFlag flag) => (self & flag) == flag;
    }
}