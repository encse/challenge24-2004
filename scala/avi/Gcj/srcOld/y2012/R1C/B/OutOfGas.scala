package y2012.R1C.B

import cmn._
import util.control.Breaks._
import scala.collection.immutable._

object OutOfGas extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    class Pos(var t: BigDecimal, var x:BigDecimal)

      val (xHome, cpos, ca) = fetch(PT3(PBigDecimal, PInt, PInt))

    var rgpos = (0 until cpos).map(_ => {
      val List(t,x) = fetch(PList(PBigDecimal))
      new Pos(t,x)
    })

    for( ipos <- 0.cond(_<cpos).next(_+1)) {
      val pos = rgpos(ipos)
      if(pos.x == xHome)
        {
          rgpos = rgpos.slice(0, ipos + 1)
          break
        }

      if(pos.x < xHome)
        {

        }
      else {
        if(ipos == 0){
          rgpos = IndexedSeq.empty[Pos]
          break
        }

        val posPrev = rgpos(ipos -1)

        pos.t = (pos.t - posPrev.t) / (pos.x - posPrev.x) * (xHome - posPrev.x) + posPrev.t

        pos.x = xHome

        rgpos = rgpos.slice(0, ipos + 1)
        break
      }
    }



      assert(rgpos.isEmpty || rgpos.last.x == xHome)

    for(a <- fetch(PList(PBigDecimal))) {
      val fCheck: BigDecimal => Boolean = tWait => (for {
        pos <- rgpos
        dt = pos.t - tWait
        if dt > 0
        xCar = 0.5 * a * dt * dt
        if xCar > pos.x
      } yield pos).isEmpty

      var tStart: BigDecimal = 0

      def avg(x: (BigDecimal, BigDecimal)) = (x._1 + x._2) / 2

      if(!fCheck(tStart))
        tStart = avg(binSearch[BigDecimal](fCheck, tStart, BigDecimal(1), BigDecimal(0.0000001)))

      out(NewLine)
      out(tStart + avg(binSearch[BigDecimal](tTotal => 0.5 * a * tTotal * tTotal < xHome, 0, 1, 0.0000001)))
    }


  }
}
