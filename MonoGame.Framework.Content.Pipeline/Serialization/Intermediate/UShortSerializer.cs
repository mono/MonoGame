// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate
{
    [ContentTypeSerializer]
    class UShortSerializer : ElementSerializer<ushort>
    {
        public UShortSerializer() :
            base("ushort", 1)
        {
        }

        protected internal override ushort Deserialize(string[] inputs, ref int index)
        {
            return XmlConvert.ToUInt16(inputs[index++]);
        }

        protected internal override void Serialize(ushort value, List<string> results)
        {
            results.Add(XmlConvert.ToString(value));
        }
    }
}