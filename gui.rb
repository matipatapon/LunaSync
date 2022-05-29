require 'ruby2d'
set title: "FOSSync"

#Requiring classes of the graphic objects !
require_relative 'go'


screen = GraphicObject.new(width: Window.width,height: Window.height)
screen.add(what: "Textobj")
screen.hide
tick = 0 

text = Textobj.new(:color => "red")

update do
  text.change_text((tick%60).to_s)
  tick+=1
end 

#Mouse interaction 
on :mouse_down do |event|
  for co in Clickable::Clickable_objects
    co.clicked(Window.mouse_x,Window.mouse_y)
  end
end

rainbowclick = Proc.new do |obj|
  puts "Fairly tail power of friendship !!!!"
  hex = ["0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F"]
  color = "#"
  6.times do 
    color+=hex[rand(15)]
  end
  obj.go.color = color
end
butt = screen.add(what: "Buttonobj",method: rainbowclick)
butt2 = butt.add(what: "Textobj",color: "white")
show
