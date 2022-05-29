require 'ruby2d'
set title: "FOSSync"

#Requiring classes of the graphic objects !
require_relative 'go'


screen = GraphicObject.new(width: Window.width,height: Window.height)

tick = 0 
update do
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
#Blueprint of the page !
puts "Width : #{Window.width} Height : #{Window.height}"
scene1 = screen.add(posx: 0,posy: 0 ,width: Window.width,height: Window.height,color: "gray")
banner = scene1.add(posx: 250,width: 140,height: 50,color: "black")
text1 = banner.add(what: "Textobj",text: "FOSSYNC",relative: true,posx: 15,posy: 10,color: "white",size: 30,style: "normal")
show 
