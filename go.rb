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

  def initialize(
    #Default parameters of GraphicObject
    posx: 0,posy: 0,height: 100,width: 100,color: "white",z: 0,
    #Textobj parameters  
    text: "whatever",size:20,style: "normal",
    #Button parameters
    click_method: nil
                )
    #Creating pre definied methods for diffrent things
    create = Proc.new do |what|
      case what
      when "rectangle" 
        @go = Rectangle.new(
        x: posx, y: posy,
        width: width, height: height, color: color , z: z
      )
      when "text"
        @go = Text.new(text, x: posx , y:posy ,color: color , size:size, z: z,style: style,text: text)
      end
    end

    standart_init = Proc.new do 
      @active = true   
      
      #Child graphic objects 
      
      @slave_go = []
    end 
    
    add_text = Proc.new do 
      puts "I am #{self} and size is #{size}"
      @text = add(what: "Textobj",color: "white",size: size,text: text)
    end

    #Child graphic objects 

    #Reporting this instance to clicable objects array 
    if self.class.include? Clickable then Clickable::Clickable_objects << self end 
    
    #personalizing initialize depending of class 
    puts "My class is #{self.class}"
    
    if self.class == Inputobj
      standart_init.call
      create.call("rectangle")
      add_text.call
    elsif self.class == Textobj 
      create.call("text")
    else self.class == GraphicObject 
      standart_init.call
      create.call("rectangle")
    end
  end    
  
  #Create new under graphical object
  def add(
    #Default arguments
    what: "GraphicObject",posx: 0,posy: 0,z: @go.z+1,height: 100,width: 100,color: "black",relative: false,object: nil,
    #Arguments for Textobj
    text: "whatever",style: "normal",size: 20,
    #Arguments for Buttonobj
    click_method: nil
    )
    new = nil
    #position relative to master object 
    if relative then posx += @go.x ; posy += @go.y end 
    case what
    when "GraphicObject"
      new = GraphicObject.new(posx: posx,posy: posy,height: height, width:width, color: color, z: z)
    when "Textobj"
      new = Textobj.new(posx: posx, posy: posy,color: color , text: text, z: z, size: size, style: style)
    when "Buttonobj"
      go = Buttonobj.new(posx: posx, posy: posy, color: color, z: z)
      go.set_click_method(click_method)
      new = go
    when "Inputobj"
      go = Inputobj.new(posx: posx,width: width,height: height, posy: posy, color: color , z: z,size: size,text: text)
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
  
  def change_text(text = "Whatever")
    posx = @go.x
    posy = @go.y
    color = @go.color
    size = @go.size  
    z = @go.z
    @go.remove 
    @go = Text.new(text,x: posx,y: posy, color: color , size: size,z: z)
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
    @text.change_text(key.to_s)
  end
end

