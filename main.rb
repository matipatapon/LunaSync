module Fs 
  #Class of the object 
  class Filedata
    def initialize(path)
      @path = path
    end
  end

  def self.get_files_from_dir(path)
    puts "Getting files from #{path}"
    Dir[path+"/*"].each do
     |path_to_file|
     file = File.new(path_to_file,"r")
     is_file = File.file?(file)
     is_dir = File.directory?(file)
     puts "Found #{path_to_file} File=#{is_file} Dir=#{is_dir}"
    end
  end

  def self.start(path1,path2)
    @files = [[],[]]
    @path1 = path1
    @path2 = path2 
    get_files_from_dir(path1)
  end
end 

Fs::start("/home/itam/Downloads","home/itam/Downloads2")


