module Clickable
  #!!!
  #! Focused object is first object in Clickable_objects

  #Clickable_objects first is focused 
  Clickable_objects = [nil,]
  #Unfocusing currently focused object 
  def self.unfocus 
    arr = Clickable_objects
    focused = arr[0]
    if focused.respond_to?(:unfocus) then focused.unfocus end 
    #If focused element isn't nil then add nil at 0 index
    if focused != nil
      arr.insert(0,nil)
    end
  end
  #Focusing curruenly focused object
  def self.focus
    focused = Clickable_objects[0]
    if focused.respond_to?(:focus) then focused.focus end 
  end

  def clicked(mx,my)
    if not @active then return end
    if @go.contains?(mx,my)  
    if @method then @method.call(self) end
    

    #If object get clicked then focus him
    arr = Clickable_objects
    arr.delete_at(arr.index(self))
    if arr[0] == nil then arr.delete_at(0) end
    arr.insert(0,self)
    end
  end
  def set_click_method method
    @method = method
  end
end

class GraphicObject
  attr_reader :go

  def initialize(posx: -1,posy: 0,height: 100,width: 100,color: "white",z: 0)
    #Creating pre definied methods for diffrent things
    create_rectangle = Proc.new do 
      puts "I am #{self}"
      @go = Rectangle.new(
      x: posx, y: posx,
      width: width, height: height, color: color , z: z
      )
    end

    standart_init = Proc.new do 
      @active = true   
      
      #Child graphic objects 
      
      @slave_go = []
    end 
    standart_init.call
    create_rectangle.call
    #Child graphic objects 

    #Reporting this instance to clicable objects array 
    if self.class.include? Clickable then Clickable::Clickable_objects << self end 
    
  end    
  
  #Create new under graphical object
  def add(what: "GraphicObject",posx: 0,posy: 0,z: @go.z+1,height: 100,width: 100,color: "black",text: "whatever",object: nil,click_method: nil)
    new = nil
    case what
    when "GraphicObject"
      new = GraphicObject.new(posx: posx,posy: posy,height: height, width:width, color: color, z: z)
    when "Textobj"
      new = Textobj.new(posx: posx, posy: posy,color: color , text: text, z: z)
    when "Buttonobj"
      go = Buttonobj.new(posx: posx, posy: posy, color: color, z: z)
      go.set_click_method(click_method)
      new = go
    when "Inputobj"
      go = Inputobj.new(posx: posx, posy: posy, color: color , z: z)
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
  
  def initialize(posx: -1,posy: 0,color: "white",text: "whatever",size:20 , z: 0)
    @active = true
    @go = Text.new(text, x: posx , y:posx ,color: color , size:size, z: z)
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

  def focus
    puts "I am the only one i am just one !"
  end

end

class Inputobj < GraphicObject 
  include Clickable 
  def get_key key
    puts key
  end
end

