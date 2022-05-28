require 'ruby2d'
set title: "FOSSync"
class Textbox
  def initialize(posx=0,posy=0,height=100,width=100,default="textbox")
    @posx = posx
    @posy = posy
    @height = height 
    @width = width
    @default=default
    @form = Rectangle.new(
      x: @posx , y: @posy,
      width: @width, height: @height,
      color: 'blue'
    )
    @text = Text.new(
      default,
      x: @posx, y: @posy,
      size: 20,
      color: 'black'
    )

  end
  def move(posx,posy)
    @form.x = posx
    @form.y = posy
    @text.x = posx
    @text.y = posx
  end
end

textbox = Textbox.new
tick = 0
update do 
  textbox.move(tick%60,tick%60)
  tick+=1
end

show
