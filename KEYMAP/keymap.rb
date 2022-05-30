#Translate input from keyboard (ruby2d) to specyfic keyboard layout 
#For example if key="a" and alt="true" then result is ą for Polish layout
#*Private*Shouldn't I use other library ? Probably ... but I must practice ruby for internship 
def translatekey(layout="PL",key,shift,alt,caps)
  file = File.new("KEYMAP/"+layout+".txt","r")
  arr = file.readlines 
  result = key
  #keypad fix
  key.sub!("keypad ","")
  
  if key == "space" then return " " end
  
  if key.length > 1 then return "" end
  
  arr.each do |line|
    #Skip comments ! 
    if line[0] == "#" then next end 
    line = line.split(" UwU ")
    if line[0] == key and (line[1] == shift.to_s or line[1] == "?") and (line[2] == alt.to_s or line[2] == "?")
      result = line[3].chomp
      break
    end
  end
  if caps 
      case shift
        when true
          result.downcase!
        when false
          result.upcase!
        end
  else
    case shift
    when true 
      result.upcase!
    end
  end
  file.close()
  return result
end
