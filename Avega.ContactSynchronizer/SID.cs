using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Avega.ContactSynchronizer {
	public class SID {
		public readonly string Value;
		public SID(string sid) {
			this.Value = sid;
		}
		public override string ToString() {
			return Value.ToString();
		}

		public static SID Find(string value) {
			Regex regex = new Regex("SID=(\\d+)", RegexOptions.IgnoreCase);
			var match = regex.Match(value);
			if (match.Success) {
				return new SID(match.Groups[1].Value);
			}

			throw new ArgumentException("Sid not found (" + value + ")");
		}
	}
}
