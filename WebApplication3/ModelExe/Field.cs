using System;
using System.Collections.Generic;
using Amazon.Textract.Model;

namespace WebApplication3.Model {

	public class Field {
		public Field(Block block, Dictionary<string, Block> blocks) {
			var relationships = block.Relationships;
			if(relationships != null && relationships.Count > 0) {
				relationships.ForEach(r => {
					if(r.Type == "CHILD") {
						this.Key = new FieldKey(block, r.Ids, blocks);
					} else if(r.Type == "VALUE") {
						r.Ids.ForEach(id => {
							var v = blocks[id];
							if(v.EntityTypes.Contains("VALUE")) {
								var vr = v.Relationships;
								if(vr != null && vr.Count > 0) {
									vr.ForEach(vc => {
										if(vc.Type == "CHILD") {
											this.Value = new FieldValue(v, vc.Ids, blocks);
										}
									});
								}
							}
						});
					}
				});
			}
		}
		public FieldKey Key { get; set; }
		public FieldValue Value { get; set; }

		public override string ToString() {
			var k = this.Key == null ? string.Empty : this.Key.ToString();
			var v = this.Value == null ? string.Empty : this.Value.ToString();
			return string.Format(@"
                {0}Field{0}===={0}
                Key: {1}, Value: {2}
            ", Environment.NewLine, k, v);
		}
	}
}