namespace PlushIT.Enums
{
    public enum Tool
    {
        SingleEdge,                 // Implemented, but may need some work down the road. Should it only add edges on click and then only subtract if user ctrl clicks?
        MultiEdge,                  // Implemented this and I don't think anyone will use it.
        MultiEdgeUserPath,          // Allow the user to embark on the worst mistake they can possibly make by allowing them to create their own path to another edge...
        MultiEdgeShortestPath,      // Allow user to click an edge and drag to another edge and have the joined via shortest path.
        Pen,                        // allow user to select multiple points and join them together via shortest path.
        EdgeSelection,              // allow user to select multiple edges
        PointSelection,             // Allow user to select multiple points
        CreateEdgePoint,            // hover over an edge and create a point that divides the triangle into two.
        CreateSurfacePoint,         // Creates point directly on the surface that divides the triangle into 3 as the three old corners draw a line to the new point.

        // Maybe tools?

        // Chainsaw                 // hack off a Selection and create another plush work file from it.
        // Cauterize                // User selects a group of points and this tool will create triangles to bridge the missing edges that are a result of the chainsaw tool.
    }
}
