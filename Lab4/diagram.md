classDiagram
    direction TB

    %% 1. СТРУКТУРА ТА ЗВ'ЯЗКИ (Оголошуємо першими для вертикального макета)
    Editor <|-- ShapeEditor
    
    Shape <|-- PointShape
    Shape <|-- LineShape
    Shape <|-- RectShape
    Shape <|-- EllipseShape
    Shape <|-- LineWithCirclesShape
    Shape <|-- CubeShape

    LineWithCirclesShape *-- LineShape : contains
    LineWithCirclesShape *-- EllipseShape : contains
    CubeShape *-- RectShape : contains
    CubeShape *-- LineShape : contains

    MainWindow --> ShapeEditor : uses
    ShapeEditor o-- Shape : contains
    ShapeEditor --> ShapeType : uses

    %% 2. ОПИС КЛАСІВ ТА МЕТОДІВ
    class Editor {
        <<abstract>>
        +OnMouseDown(canvas: Canvas, e: MouseButtonEventArgs) void
        +OnMouseMove(canvas: Canvas, e: MouseEventArgs) void
        +OnMouseUp(canvas: Canvas, e: MouseButtonEventArgs) void
    }

    class ShapeEditor {
        -pcshape: List
        -isDrawing: bool
        -tempShape: Shape
        +CurrentShapeType: ShapeType
        +OnMouseDown(canvas: Canvas, e: MouseButtonEventArgs) void
        +OnMouseMove(canvas: Canvas, e: MouseEventArgs) void
        +OnMouseUp(canvas: Canvas, e: MouseButtonEventArgs) void
    }

    class ShapeType {
        <<enumeration>>
        Point
        Line
        Rect
        Ellipse
        LineWithCircles
        Cube
    }

    class Shape {
        <<abstract>>
        #x1, y1, x2, y2: double
        +Set(startX: double, startY: double, endX: double, endY: double) void
        +CreateElement()* UIElement
        +Update(element: UIElement)* void
        +Draw(canvas: Canvas) void
        +ApplyRubberStyleToElement(element: UIElement) void
    }

    class PointShape {
        +CreateElement() UIElement
        +Update(element: UIElement) void
    }

    class LineShape {
        +CreateElement() UIElement
        +Update(element: UIElement) void
    }

    class RectShape {
        +CreateElement() UIElement
        +Update(element: UIElement) void
    }

    class EllipseShape {
        +CreateElement() UIElement
        +Update(element: UIElement) void
    }

    class LineWithCirclesShape {
        -_lineShape: LineShape
        -_ellipse1: EllipseShape
        -_ellipse2: EllipseShape
        +CreateElement() UIElement
        +Update(element: UIElement) void
    }

    class CubeShape {
        -_frontRect: RectShape
        -_backRect: RectShape
        -_lines: LineShape[]
        +CreateElement() UIElement
        +Update(element: UIElement) void
    }

    class MainWindow {
        -myEditor: ShapeEditor$
        +MainWindow()
        -SetShapeType(type: ShapeType) void
    }
