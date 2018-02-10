using Neat;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class GenotypeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public Genome Genome { get; set; }

        public List<ConnectionGene> EnabledConnectionGenes
        {
            get
            {
                List<ConnectionGene> enabledConnectionGenes = new List<ConnectionGene>(Genome.ConnectionGenes);
                enabledConnectionGenes.RemoveAll(connectionGene => !connectionGene.IsEnabled);
                return enabledConnectionGenes;
            }
        }

        public List<NeuronGene> SortedNeuronGenes {
            get
            {
                List<NeuronGene> sortedList = new List<NeuronGene>(Genome.NeuronGenes);
                sortedList.Sort((neuronGene1, neuronGene2) => neuronGene1.CompareTo(neuronGene2));
                return sortedList;
            }
        }

        public int CountHiddenNeuronGenes
        {
            get
            {
                return Genome.NeuronGenes.FindAll(neuronGene => neuronGene.Type == NeuronType.Hidden).Count;
            }
        }

        public GenotypeViewModel(Genome genome)
        {
            Genome = genome;
        }

        // NotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
