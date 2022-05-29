module Clickable
  Clickable_objects = []
  def clicked(mx,my)
    if @go.contains?(mx,my) and @active then @method.call(self) end
  end
  def set_click_method method
    @method = method
  end
end

class GraphicObject
  attr_reader :go

  def initialize(posx: -1,posy: 0,height: 100,width: 100,color: "white")
    
    @active = true
    
    @go = Rectangle.new(
    x: posx, y: posx,
    width: width, height: height, color: color
    )

    #Child graphic objects 
    @slave_go = [] 

    #Reporting this instance to clicable objects array 
    if self.class.include? Clickable then Clickable::Clickable_objects << self end 
    
  end    
  
  #Create new under graphical object
  def add(what: "GraphicObject",posx: 0,posy: 0,z: @go.z+1,height: 100,width: 100,color: "black",text: "whatever",object: nil,method: nil)
    new = nil
    case what
    when "GraphicObject"
      new = GraphicObject.new(posx: posx,posy: posy,height: height, width:width, color: color)
    when "Textobj"
      new = Textobj.new(posx: posx, posy: posy,color: color , text: text)
    when "Buttonobj"
      go = Buttonobj.new(posx: posx, posy: posy, color: color)
      go.set_click_method(method)
      new = go
    end
    return new
    
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
    @go = Text.new(text, x: posx , y:posx ,color: color , size:size)
    @slave_go = []
  end
  
  def change_text(text = "Whatever")
    posx = @go.x
    posy = @go.y
    color = @go.color
    size = @go.size  
    @go.remove 
    @go = Text.new(text,x: posx,y: posy, color: color , size: size)
  end
end

class Buttonobj < GraphicObject
  include Clickable
end
