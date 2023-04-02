using System;
using System.Collections.Generic;
using Amazon.Textract.Model;

/*
class Line:
    def __init__(self, block, blockMap):

        self._block = block
        self._confidence = block['Confidence']
        self._geometry = Geometry(block['Geometry'])
        self._id = block['Id']

        self._text = ""
        if(block['Text']):
            self._text = block['Text']

        self._words = []
        if('Relationships' in block and block['Relationships']):
            for rs in block['Relationships']:
                if(rs['Type'] == 'CHILD'):
                    for cid in rs['Ids']:
                        if(blockMap[cid]["BlockType"] == "WORD"):
                            self._words.append(Word(blockMap[cid], blockMap))
    def __str__(self):
        s = "Line\n==========\n"
        s = s + self._text + "\n"
        s = s + "Words\n----------\n"
        for word in self._words:
            s = s + "[{}]".format(str(word))
        return s

    @property
    def confidence(self):
        return self._confidence

    @property
    def geometry(self):
        return self._geometry

    @property
    def id(self):
        return self._id

    @property
    def words(self):
        return self._words

    @property
    def text(self):
        return self._text

    @property
    def block(self):
        return self._block
 */

namespace  WebApplication3.Model {
	public class Line {
		public Line(Block block, Dictionary<string, Block> blocks) {
			this.Block = block;
			this.Confidence = block.Confidence;
			this.Geometry = block.Geometry;
			this.Id = block.Id;
			this.Text = block == null ? string.Empty : block.Text;
			this.Words = new List<Word>();

			var relationships = block.Relationships;
			if(relationships != null && relationships.Count > 0) {
				relationships.ForEach(r => {
					if(r.Type == "CHILD") {
						r.Ids.ForEach(id => {
                            var block = blocks[id];
                            if(block.BlockType == "WORD")
                                this.Words.Add(new Word(block, blocks));
                            else
                                this.Words.Add(new Word(null, blocks));
                        });
					}
				});
			}
		}

		public float Confidence { get; set; }
		public Geometry Geometry { get; set; }
		public string Id { get; set; }
		public List<Word> Words { get; set; }
		public string Text { get; set; }
		public Block Block { get; set; }

		public override string ToString() {
			return string.Format(@"
                Line{0}===={0}
                {1} {0}
                Words{0}----{0}
                {2}{0}
                ----
            ", Environment.NewLine, this.Text, string.Join(", ", this.Words));
		}
	}
}