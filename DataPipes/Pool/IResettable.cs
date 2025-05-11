using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataPipes.Pool;
/// <summary>Interface for the list of pools</summary>
public interface IResettable
{
    /// <summary>Method that updates information to base values</summary>
    void Reset();
}
