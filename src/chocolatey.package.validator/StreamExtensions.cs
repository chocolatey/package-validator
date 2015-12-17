// Copyright © 2015 - Present RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// 	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.package.validator
{
    using System.IO;
    using System.Text;

    public static class StreamExtensions
    {
        // Adapted from blog post
        // https://weblog.west-wind.com/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader
        public static Encoding get_encoding(this Stream file)
        {
            // *** Use Default of Encoding.Default (Ansi CodePage)
            var enc = Encoding.Default;

            // *** Detect byte order mark if any - otherwise assume default
            var buffer = new byte[5];
            file.Read(buffer, 0, 5);
            file.Close();

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf) enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff) enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff) enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76) enc = Encoding.UTF7;

            return enc;
        }
    }
}
