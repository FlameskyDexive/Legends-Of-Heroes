namespace ET.Client
{
    /// <summary>
    /// Text 一行的顶点范围描述：起点 / 终点索引 + 顶点数。
    /// 配合 <see cref="TextSpacing"/> 用于调整一行内字符之间的额外间距。
    /// </summary>
    public class Line
    {
        private readonly int _startVertexIndex;
        private readonly int _endVertexIndex;
        private readonly int _vertexCount;

        public int StartVertexIndex => _startVertexIndex;
        public int EndVertexIndex => _endVertexIndex;
        public int VertexCount => _vertexCount;

        public Line(int startVertexIndex, int length)
        {
            _startVertexIndex = startVertexIndex;
            _endVertexIndex = length * 6 - 1 + startVertexIndex;
            _vertexCount = length * 6;
        }
    }
}
