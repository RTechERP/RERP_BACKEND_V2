namespace RERPAPI.Model.Param
{
    public class ProjectTaskTreeNodeParam
    {
        public spGetProjectTaskTreeParam Data { get; set; }
        public List<ProjectTaskTreeNodeParam> Children { get; set; }
        public bool Leaf { get; set; }
        public bool Expanded { get; set; }
    }
}