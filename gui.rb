require 'ruby2d'
set title: "FOSSync"

#Requiring classes of the graphic objects !
require_relative 'go'


screen = GraphicObject.new(width: Window.width,height: Window.height)
screen.add(what: "Textobj")
tick = 0 

text = Textobj.new(:color => "red")

update do
  text.change_text((tick%60).to_s)
  tick+=1
end 

#Mouse interaction 
on :mouse_down do |event|
  Clickable::unfocus
  for co in Clickable::Clickable_objects
    if co.respond_to?(:clicked) then co.clicked(Window.mouse_x,Window.mouse_y) end
  end
  Clickable::focus
end
#Keyboard interaction 
on :key_down do |event|
  key = event.key
  #transmit keys do current focused element if he has get_key method !!!
  focused = Clickable::Clickable_objects[0]
  if focused.respond_to?(:get_key) then focused.get_key(key) end

end
#Test zone 
rainbowclick = Proc.new do |obj|
  puts "Fairly tail power of friendship !!!!"
  hex = ["0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F"]
  color = "#"
  6.times do 
    color+=hex[rand(15)]
  end
  obj.go.color = color
end
butt = screen.add(what: "Inputobj",z: 10)
show
