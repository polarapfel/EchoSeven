require 'open3'

desc 'Initialize and install build dependencies. Run with sudo'

task :init do
  stdout, stderr, status = Open3.capture3('cat /etc/debian_version')
  if status != 0
    puts "This doesn't seem to be a Debian based Linux distribution. Exiting..."
    exit(1)
  end

  _debian_variant = ''
  _debian_version = ''

  stdout, stderr, status = Open3.capture3('lsb_release -d')
  if stdout.include? 'Ubuntu'
    puts 'Found Ubuntu.'
    _debian_variant = 'Ubuntu'
  end
  if stdout.include? 'Debian'
    puts 'Found Debian.'
    _debian_variant = 'Debian'
  end

  stdout, stderr, status = Open3.capture3('lsb_release -sr')
  puts 'Version: ' + stdout.chomp
  _debian_version = String.new(stdout).chomp

  if _debian_variant == 'Debian'
    case _debian_version
    when '10'
      sh 'wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb'
      sh 'sudo dpkg -i /tmp/packages-microsoft-prod.deb'
    when '9'
      sh 'wget -O - https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > /tmp/microsoft.asc.gpg'
      sh 'sudo mv /tmp/microsoft.asc.gpg /etc/apt/trusted.gpg.d/'
      sh 'wget https://packages.microsoft.com/config/debian/9/prod.list -O /tmp/microsoft-prod.list'
      sh 'sudo mv /tmp/microsoft-prod.list /etc/apt/sources.list.d/microsoft-prod.list'
      sh 'sudo chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg'
      sh 'sudo chown root:root /etc/apt/sources.list.d/microsoft-prod.list'
    else
      puts 'Unsupported Debian version. Exiting.'
      exit(1)
    end
  end

  if _debian_variant == 'Ubuntu'
    case _debian_version
    when '20.04'
      sh 'wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb'
      sh 'sudo dpkg -i /tmp/packages-microsoft-prod.deb'
    when '18.04'
      sh 'wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb'
      sh 'sudo dpkg -i /tmp/packages-microsoft-prod.deb'
    else
      puts 'Unsupported Ubuntu version. Exiting.'
      exit(1)
    end
  end
  sh 'sudo apt-get update && sudo apt-get install -y debhelper dh-make debmake debmake-doc gnupg lintian gzip apt-transport-https dotnet-sdk-3.1'
end
