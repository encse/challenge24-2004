package y2012.R1B.B

import cmn._

import scala.collection.mutable

object TideGoesInTideGoesOut extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out): Unit = {
    class Cave(val x: Int, val y: Int, val plafon: Int, val padlo: Int) {
      var dist: BigDecimal = null

      def ensureDist(newDist: BigDecimal) : this.type = {
        dist = if(dist == null) newDist else dist.min(newDist)
        this
      }
    }

    val (hStart, ymax, xmax) = fetch(PT3(PBigDecimal, PInt, PInt))

    val mcave = (0 until ymax).map(y => (y, fetch(PIndexedSeq(PInt)))).map{
      case (y, rgplafon) => rgplafon.zip(fetch(PList(PInt))).zipWithIndex.map{
        case ((plafon, padlo), x) => new Cave(x,y,plafon,padlo)
      }
    }

    val caveStart = mcave(0)(0).ensureDist(0)
    val caveEnd = mcave(ymax - 1)(xmax - 1)

    def calcDist(caveFrom: Cave, caveTo: Cave) : Option[BigDecimal] = {
      if(caveTo.plafon - caveTo.padlo < 50
        || caveFrom.plafon - caveTo.padlo < 50
        || caveTo.plafon - caveFrom.padlo < 50)
        return None

      var t = caveFrom.dist

      var h = (hStart - t * 10).max(0)
      assert(caveFrom.plafon - h >= 50)
      if(caveTo.plafon - h < 50)
      {
        val hOk = caveTo.plafon - 50
        t += (h - hOk) / 10
        h = hOk
      }
      assert(caveTo.plafon - h >= 50)

      if(t == 0)
      {
        assert(h == hStart)
        t += 0
      }
      else if(h - caveFrom.padlo < 20)
        t += 10
      else
        t += 1
      Some(t)

    }

    out(astar[Cave,BigDecimal](
      Some(caveStart),
      _ eq caveEnd,
      caveFrom => List(
        (1,0),
        (-1,0),
        (0, 1),
        (0, -1)
      ).map{
        case (dx,dy) => (caveFrom.x+dx, caveFrom.y+dy)
      }.filter {
        case (x,y) => 0 <= x && x < xmax && 0<= y && y < ymax
      }.map {
        case (x,y) => {
          val cave = mcave(y)(x)
          (cave, calcDist(caveFrom, cave))
        }
      }.filter {
        case (cave, odist) => odist.isDefined
      }.map {
        case (cave, odist) => cave.ensureDist(odist.get)
      },
      _.dist
    ).get._2)


  }
}
