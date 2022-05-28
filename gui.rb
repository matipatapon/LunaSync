require 'ruby2d'
set title: "FOSSync"

#Requiring classes of the graphic objects !
require_relative 'go'


screen = GraphicObject.new(width: Window.width,height: Window.height)
screen.add(what: "Textobj")
tick = 0 
button = Buttonobj.new
update do
  case 
  when (tick%60) === 0
    screen.hide
  when (tick%60) === 30
    screen.show
  end
  tick+=1
end 
show
