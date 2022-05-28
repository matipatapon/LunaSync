class GraphicObject
  def initialize(posx: -1,posy: 0,height: 100,width: 100,color: "white")
    p self
    @active = true
    @go = Rectangle.new(
    x: posx, y: posx,
    width: width, height: height, color: color
    ) #Child graphic object 
    @slave_go = [] 
  end    
  
  #Create new under graphical object
  def add(what: "GraphicObject",posx: 0,posy: 0,height: 100,width: 100,color: "black",text: "whatever")
    case what
    when "GraphicObject"
      @slave_go << GraphicObject.new(posx: posx,posy: posy,height: height, width:width, color: color)
    when "Textobj"
      @slave_go << Textobj.new(posx: posx, posy: posy,color: color , text: text)
    end
    
  end

  
  #move to exact location on the screen 
  def move_to(posx,posy)
    rx = posx - @go.x
    ry = posy - @go.y 
    move(rx,ry)
  end

  
  #move by rx and ry pixels 
  def move(rx,ry)
    @go.x+=rx
    @go.y+=ry
    for go in @slave_go
      go.move(rx,ry)
    end
  end
  
  def hide
    @active = false
    @go.remove
    for go in @slave_go
      go.hide
    end
  end

  def show
    @active = true
    @go.add
    for go in @slave_go
      go.show
    end
  end

end

#Text object
class Textobj < GraphicObject 
  def initialize(posx: -1,posy: 0,color: "white",text: "whatever",size:20)
    @active = true 
    @go = Text.new(
      text, x: posx , y:posx ,color: color , size:size)
    @slave_go = []
    
  end
end
